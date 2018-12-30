namespace Kopcha.Models
{
    public class CaptchaField
    {
        public CaptchaType FieldType { get; set; }
        public int Threshold { get; set; } = -1;
        public int Duration { get; set; } = -1;
        public string FieldValue { get; set; }
        public bool IsEnabledThreshold { get; set; }
        public bool IsEnabledDuration { get; set; }
    }
}