public enum Language
{
    ENG,
    CHN,
}

public static class LanguageExtension
{
    public static string ToPrettyString(this Language lan)
    {
        switch (lan)
        {
            case Language.ENG:
                return "英文";
            case Language.CHN:
                return "中文";
            default:
                return "Unknown";
        }
    }
}