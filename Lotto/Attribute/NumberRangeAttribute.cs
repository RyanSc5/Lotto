using System.ComponentModel.DataAnnotations;

namespace Lotto.Attribute
{
    public class NumberRangeAttribute : ValidationAttribute
    {
        private readonly int _min;
        private readonly int _max;

        // 設定屬性允許的最小和最大值
        public NumberRangeAttribute(int min, int max)
        {
            _min = min;
            _max = max;
        }

        // 覆寫驗證方法
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (value is List<int> numbers)  // 檢查屬性是否為 List<int>
            {
                foreach (var number in numbers)
                {
                    if (number < _min || number > _max)  // 如果數字超出範圍
                    {
                        return new ValidationResult(ErrorMessage ?? $"每個號碼必須在 {_min} 到 {_max} 之間");
                    }
                }
            }
            return ValidationResult.Success;  // 如果所有數字均有效，返回成功
        }
    }
}
