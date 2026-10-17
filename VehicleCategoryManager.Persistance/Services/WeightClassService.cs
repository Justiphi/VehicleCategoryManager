using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;
using VehicleCategoryManager.Persistance.Models;

namespace VehicleCategoryManager.Persistance.Services
{
    public class WeightClassService : ServiceBase
    {
        public WeightClassService(DataContextFactory contextFactory) : base(contextFactory) { }

        private List<string> Icons =
        [
            "icons/light-weight.svg",
            "icons/med-weight.svg",
            "icons/heavy-weight.svg",
            "icons/weight.svg",
            "icons/weight2.svg",
            "icons/weight3.svg",
            "icons/weight4.svg",
            "icons/weight5.svg"
        ];

        private bool ValidateWeightClass(DataContext context, VehicleWeightClass weightClass)
        {
            //cannot add or update a weight class to cover the entire range of weights, this can only exist by deleting all other weight classes.
            if (weightClass.MinWeightKg == 0 && weightClass.MaxWeightKg == null)
            {
                return false;
            }

            //cannot add a icon that does not exist
            if(!Icons.Contains(weightClass.VehicleWeightClassIconPath))
            {
                return false;
            }

            var classes = context.VehicleWeightClasses.Where(wc => wc.VehicleWeightClassId != weightClass.VehicleWeightClassId);

            //duplication of name is not allowed
            if (classes.Any(wc => wc.VehicleWeightClassName == weightClass.VehicleWeightClassName))
            {
                return false;
            }

            //duplication of icons not allowed
            if (classes.Any(wc => wc.VehicleWeightClassIconPath == weightClass.VehicleWeightClassIconPath))
            {
                return false;
            }

            //if a weight class already exists within the same min and max weight, do not allow add
            if (classes.Any(wc => wc.MinWeightKg == weightClass.MinWeightKg && wc.MaxWeightKg == weightClass.MaxWeightKg))
            {
                return false;
            }

            //if a weight class fully encloses the new weight class, do not allow add
            if (classes.Any(wc => wc.MinWeightKg > weightClass.MinWeightKg && (weightClass.MaxWeightKg == null || wc.MaxWeightKg < weightClass.MaxWeightKg)))
            {
                return false;
            }

            //if an existing weight class is fully enclosed by the new weight class, do not allow add
            if (classes.Any(wc => wc.MinWeightKg > weightClass.MinWeightKg && ((weightClass.MaxWeightKg == null && wc.MaxWeightKg == null) || wc.MaxWeightKg > weightClass.MaxWeightKg)))
            {
                return false;
            }

            return true;
        }

        public int AddWeightClass(VehicleWeightClass weightClass)
        {
            using (var context = _contextFactory.CreateContext())
            {

                if (!ValidateWeightClass(context, weightClass))
                {
                    return 0;
                }

                AdjustSideClassesToFixGaps(context, weightClass);

                context.VehicleWeightClasses.Add(weightClass);
                context.SaveChanges();
            }

            return weightClass.VehicleWeightClassId;
        }

        public bool UpdateWeightClass(VehicleWeightClass weightClass)
        {
            VehicleWeightClass? weightClassInstanced = null;

            using (var context = _contextFactory.CreateContext())
            {

                if (!ValidateWeightClass(context, weightClass))
                {
                    return false;
                }

                weightClassInstanced = context.VehicleWeightClasses.Find(weightClass.VehicleWeightClassId);

                //ensure record exists before updating
                if (weightClassInstanced == null)
                {
                    return false;
                }

                weightClassInstanced.VehicleWeightClassName = weightClass.VehicleWeightClassName;
                weightClassInstanced.VehicleWeightClassIconPath = weightClass.VehicleWeightClassIconPath;
                weightClassInstanced.MinWeightKg = weightClass.MinWeightKg;
                weightClassInstanced.MaxWeightKg = weightClass.MaxWeightKg;

                context.VehicleWeightClasses.Update(weightClassInstanced);
                AdjustSideClassesToFixGaps(context, weightClass);

                context.SaveChanges();
            }

            return true;
        }

        /// <summary>
        /// adjusts the min and max weight of the previous and next weight classes to ensure there are no gaps between weight classes after an update.
        /// </summary>
        /// <param name="context">datbase context</param>
        /// <param name="weightClass">updated weight class object</param>
        /// <returns>success status</returns>
        private bool AdjustSideClassesToFixGaps(DataContext context, VehicleWeightClass weightClass)
        {            
            var classes = context.VehicleWeightClasses.Where(wc => wc.VehicleWeightClassId != weightClass.VehicleWeightClassId);

            //new first class
            if (weightClass.MinWeightKg == 0)
            {
                var firstClass = classes.OrderBy(wc => wc.MinWeightKg).FirstOrDefault();
                if (firstClass != null && weightClass.MaxWeightKg != null)
                {
                    firstClass.MinWeightKg = weightClass.MaxWeightKg.Value;
                    context.VehicleWeightClasses.Update(firstClass);
                }
            }
            else
            {
                //if this is a new first class, there is no previous class to adjust
                var previousClass = classes.Where(wc => wc.MaxWeightKg > weightClass.MinWeightKg).OrderByDescending(wc => wc.MaxWeightKg).FirstOrDefault();
                if (previousClass != null)
                {
                    previousClass.MaxWeightKg = weightClass.MinWeightKg;
                    context.Update(previousClass);
                }
            }

            //new last class
            if(weightClass.MaxWeightKg == null)
            {
                var lastClass = classes.Where(wc => wc.MaxWeightKg == null).FirstOrDefault();
                if (lastClass != null)
                {
                    lastClass.MaxWeightKg = weightClass.MinWeightKg;
                    context.VehicleWeightClasses.Update(lastClass);
                }
            }
            else
            {
                //if this is a new last class, there is no next class to adjust
                var nextClass = classes.Where(wc => wc.MinWeightKg < weightClass.MaxWeightKg).OrderByDescending(wc => wc.MinWeightKg).FirstOrDefault();
                if (nextClass != null)
                {
                    nextClass.MinWeightKg = weightClass.MaxWeightKg.Value;
                    context.Update(nextClass);
                }
            }

            return true;
        }

        public bool DeleteWeightClass(int weightClassId)
        {
            using (var context = _contextFactory.CreateContext())
            {

                //cannot delete last record, must have at least one weight class
                if (context.VehicleWeightClasses.Count() == 1)
                {
                    return false;
                }

                var weightClass = context.VehicleWeightClasses.Find(weightClassId);
                if (weightClass == null)
                {
                    return false;
                }

                decimal? midPoint = weightClass.MinWeightKg == 0 ? 0 : weightClass.MaxWeightKg == null ? null : (weightClass.MinWeightKg + weightClass.MaxWeightKg) / 2;

                context.VehicleWeightClasses.Remove(weightClass);
                if(!FillWeightClassGaps(context, weightClassId, midPoint))
                {
                    return false;
                }
                context.SaveChanges();
            }

            return true;
        }

        /// <summary>
        /// Fills the gaps between weight classes based on the provided join point. 
        /// If there is only one weight class, it sets the minimum of the first weight class to 0 and the maximum to null. 
        /// If the join point is 0, it sets the minimum of the first weight class to 0. 
        /// If the join point is null, it sets the maximum of the last weight class to null. 
        /// If the join point is in the middle of two weight classes, it sets the maximum of the lower weight class and the minimum of the higher weight class to the join point.
        /// </summary>
        /// <param name="context"></param>
        /// <param name="joinPoint"></param>
        /// <returns></returns>
        private bool FillWeightClassGaps(DataContext context, int excludeId, decimal? joinPoint)
        {
            VehicleWeightClass? weightClass = null;

            if (joinPoint < 0)
            {
                return false;
            }

            //last weight class, but only one weight class exists, set the minimum of the first weight class to 0 and the maximum to null
            if (context.VehicleWeightClasses.Where(wc => wc.VehicleWeightClassId != excludeId).Count() == 1)
            {
                weightClass = context.VehicleWeightClasses.Where(wc => wc.VehicleWeightClassId != excludeId).First();
                weightClass.MinWeightKg = 0;
                weightClass.MaxWeightKg = null;
                context.VehicleWeightClasses.Update(weightClass);
                return true;
            }

            //new minimym weight class, set the minimum of the first weight class to 0
            if (joinPoint == 0)
            {
                weightClass = context.VehicleWeightClasses.Where(wc => wc.VehicleWeightClassId != excludeId).OrderBy(wc => wc.MinWeightKg).First();
                weightClass.MinWeightKg = 0;
                context.VehicleWeightClasses.Update(weightClass);

                return true;
            }

            //new maximum weight class, set the maximum of the last weight class to null
            if (joinPoint == null)
            {
                weightClass = context.VehicleWeightClasses.Where(wc => wc.VehicleWeightClassId != excludeId).OrderByDescending(wc => wc.MaxWeightKg).First();
                weightClass.MaxWeightKg = null;
                context.VehicleWeightClasses.Update(weightClass);
                return true;
            }

            //join point is in the middle of two weight classes, set the maximum of the lower weight class and the minimum of the higher weight class to the join point
            weightClass = context.VehicleWeightClasses.Where(wc => wc.VehicleWeightClassId != excludeId && wc.MinWeightKg > joinPoint).OrderByDescending(wc => wc.MinWeightKg).First();
            weightClass.MinWeightKg = joinPoint.Value;
            context.VehicleWeightClasses.Update(weightClass);

            weightClass = context.VehicleWeightClasses.Where(wc => wc.VehicleWeightClassId != excludeId && wc.MaxWeightKg < joinPoint).OrderBy(wc => wc.MaxWeightKg).First();
            weightClass.MaxWeightKg = joinPoint.Value;
            context.VehicleWeightClasses.Update(weightClass);

            return true;
        }

        public VehicleWeightClass? GetWeightClassById(int weightClassId)
        {
            VehicleWeightClass? weightClass = null;

            using (var context = _contextFactory.CreateContext())
            {
                weightClass = context.VehicleWeightClasses.Find(weightClassId);
            }

            //if not found, will return null
            return weightClass;
        }

        public VehicleWeightClass? GetWeightClassByWeight(decimal weight)
        {
            VehicleWeightClass? weightClass = null;

            using (var context = _contextFactory.CreateContext())
            {
                weightClass = context.VehicleWeightClasses.FirstOrDefault(wc => wc.MinWeightKg <= weight && (wc.MaxWeightKg == null || wc.MaxWeightKg > weight));
            }

            //if not found, will return null
            return weightClass;
        }

        public IEnumerable<VehicleWeightClass> GetWeightClassList()
        {
            List<VehicleWeightClass> weightClasses = new List<VehicleWeightClass>();

            using (var context = _contextFactory.CreateContext())
            {
                weightClasses = context.VehicleWeightClasses.ToList();
            }

            //if not found, will return null
            return weightClasses;
        }
    }
}
