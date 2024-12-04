using ReceitasAllAPI.Entities;

namespace ReceitasAllAPI.Dtos
{
    public class IngredientResponseDto
    {
        public int ID { get; set; }
        public int Order { get; set; }
        public string Value { get; set; }

        public static IngredientResponseDto FromEntity(Ingredient step)
        {
            if (step == null)
            {
                return null;
            }

            return new IngredientResponseDto
            {
                ID = step.ID,
                Order = step.Order,
                Value = step.Value
            };
        } 

    }
}