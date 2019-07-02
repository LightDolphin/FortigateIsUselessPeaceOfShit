using Profile.Model.Models;
using System;
using System.Collections.Generic;

namespace Profile.Web.ViewModels
{
    public class HRStudentDataViewModel
    {
        public List<TableRow> TableData { get; set; }
        public IEnumerable<LanguageLevel> LanguageLevels { get; set; }
        public IEnumerable<Stream> Streams { get; set; }
        public class TableRow
        {
            public int StudentId { get; set; }
            public bool Choose { get; set; }

            public string Status { get; set; }

            public Stream Stream { get; set; }

            public string RuName { get; set; }

            public string RuSurname { get; set; }

            public DateTime DateOfBirth { get; set; }

            public string Email { get; set; }

            public string Phone { get; set; }

            public string TrainerEmail { get; set; }

            public DateTime DateOfGraduation { get; set; }

            public int GraduationMark { get; set; }

            public bool ParticipationOnPracticalLab { get; set; }

            public IEnumerable<string> StudentSkills { get; set; }

            public IEnumerable<ForeignLanguage> Languages { get; set; }

            public IEnumerable<string> Specializations { get; set; }

            public string Comments { get; set; }
        }
    }
}