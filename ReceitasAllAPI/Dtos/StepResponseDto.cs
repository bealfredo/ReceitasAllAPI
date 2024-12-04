using ReceitasAllAPI.Entities;

namespace ReceitasAllAPI.Dtos
{
    public class StepResponseDto
    {
        public int ID { get; set; }
        public int Order { get; set; }
        public string Value { get; set; }

        public static StepResponseDto FromEntity(Step step)
        {
            if (step == null)
            {
                return null;
            }

            return new StepResponseDto
            {
                ID = step.ID,
                Order = step.Order,
                Value = step.Value
            };
        } 

    }
}