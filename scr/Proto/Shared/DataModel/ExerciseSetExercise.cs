using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace DataModel
{
    public class ExerciseSetExercise
    {
        public long Id
        {
            get => default;
            set
            {
            }
        }

        [ForeignKey(nameof(ExerciseSet))]
        public long ExerciseSetId
        {
            get => default;
            set
            {
            }
        }

        [ForeignKey(nameof(Exercise))]
        public long ExerciseId
        {
            get => default;
            set
            {
            }
        }
    }
}
