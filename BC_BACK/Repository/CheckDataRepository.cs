using BC_BACK.Dto;
using BC_BACK.Interfaces;
using System.Text.RegularExpressions;

namespace BC_BACK.Repository
{
    public class CheckDataRepository : ICheckDataRepository
    {
        private readonly Regex _slovakWordRegex;
        public CheckDataRepository() 
        {
            _slovakWordRegex = new Regex(@"^[\p{L}]+$", RegexOptions.IgnoreCase);
        }
        public bool CheckStringLengs(string word, int length)
        {
            if (word.Length <= length) { return true; }
            return false;
        }

        public bool IsSlovakWord(string input)
        {
            if (string.IsNullOrWhiteSpace(input))
                return false;
            return _slovakWordRegex.IsMatch(input);
        }

        public string CheckUser(UserDto user)
        {
            throw new NotImplementedException();
        }
    }
}
