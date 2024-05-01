using BC_BACK.Dto;

namespace BC_BACK.Interfaces
{
    public interface ICheckDataRepository
    {
        bool CheckStringLengs(string word, int length);
        bool IsSlovakWord(string input);
    }
}
