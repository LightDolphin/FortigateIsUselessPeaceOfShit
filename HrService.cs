using Profile.Model.Models;
using Profile.Service.Services.Interfaces;
using Profile.Web.Services.Interfaces;
using Profile.Web.ViewModels;
using System.Collections.Generic;
using System.Linq;

namespace Profile.Web.Services
{
    public class HrService : IHrService
    {
        private readonly IStudentService _studentService;
        private readonly IStreamService _streamService;
        private readonly IResumeService _resumeService;
        private readonly IUserInfoService _userInfoService;
        private readonly ISummaryService _summaryService;
        private readonly ISkillService _skillService;
        private readonly IForeignLanguageService _languageService;
        private readonly IEducationService _educationService;
        private readonly ICourseService _courseService;
        private readonly ICertificateService _certificateService;
        private readonly IExamService _examService;
        private readonly IWorkExperienceService _workExperienceService;
        private readonly IPortfolioService _portfolioService;
        private readonly IMilitaryStatusService _militaryStatusService;
        private readonly IRecommendationService _recommendationService;
        private readonly IAdditionalInfoService _additionalInfoService;
        public HrService(
            IStudentService studentService, IResumeService resumeService, IUserInfoService userInfoService,
                                IStreamService streamService, ISummaryService summaryService,
                                ISkillService skillService, IForeignLanguageService languageService, IEducationService educationService,
                                ICourseService courseService, ICertificateService certificateService, IExamService examService,
                                IWorkExperienceService workExperienceService, IPortfolioService portfolioService, IMilitaryStatusService militaryStatusService,
                                IRecommendationService recommendationService, IAdditionalInfoService additionalInfoService
            )
        {
            _userInfoService = userInfoService;
            _studentService = studentService;
            _resumeService = resumeService;
            _streamService = streamService;
            _summaryService = summaryService;
            _skillService = skillService;
            _languageService = languageService;
            _educationService = educationService;
            _courseService = courseService;
            _certificateService = certificateService;
            _examService = examService;
            _workExperienceService = workExperienceService;
            _portfolioService = portfolioService;
            _militaryStatusService = militaryStatusService;
            _additionalInfoService = additionalInfoService;
            _recommendationService = recommendationService;
        }

        public HRStudentDataViewModel GetAllStudentViewData()
        {
            IList<Student> students = _studentService.GetAllStudents();

            if (students.Count == 0)
            {
                return null;
            }

            HRStudentDataViewModel viewModel = new HRStudentDataViewModel
            {
                LanguageLevels = _languageService.GetAllLanguageLevels(),
                Streams = _streamService.GetAllStreams(),
                TableData = new List<HRStudentDataViewModel.TableRow>()
            };

            foreach (Student st in students)
            {
                UserInfo studentInfo = _studentService.GetUserInfo(st);
                Resume resume = _resumeService.GetResumeByStudentId(st.Id);
                HRStudentDataViewModel.TableRow row = new HRStudentDataViewModel.TableRow()
                {
                    StudentId = st.Id,
                    Choose = false,
                    Stream = _streamService.GetStreamByStudentId(st.Id),
                    RuName = studentInfo?.RuName,
                    RuSurname = studentInfo.RuSurname,
                    DateOfBirth = studentInfo.DateOfBirth,
                    Email = studentInfo.Email,
                    Phone = studentInfo.Phone,
                    TrainerEmail = "trainer@profile.com",
                    DateOfGraduation = st.DateOfGraduation,
                    GraduationMark = st.GraduationMark,
                    ParticipationOnPracticalLab = false,
                    Comments = null
                };

                if (resume == null)
                {
                    row.Languages = null;
                    row.Status = "new";
                    row.Specializations = null;
                }
                else
                {
                    row.Languages = _languageService.GetAllForeignLanguagesForResume(resume.Id);
                    row.Status = resume.Status;
                    row.Specializations = resume.Educations?.Select(s => s.Specialization);
                }

                var skills = _skillService.GetAllSkillsForStudent(st.Id);
                if (skills.Count() == 0)
                {
                    row.StudentSkills = _skillService.GetDefaultSkillsByStream(_streamService.GetStreamByStudentId(st.Id).Id);
                }
                else
                {
                    row.StudentSkills = skills;
                }
                viewModel.TableData.Add(row);
            }
            return viewModel;
        }

        public ResumeViewModel GetFullResumeInfoByStudentId(int id)
        {
            Resume resume = _resumeService.GetResumeByStudentId(id);
            if (resume == null)
                return null;
            ResumeViewModel vm = new ResumeViewModel()
            {
                UserInfo = _userInfoService.GetUserInfoByResume(resume),
                ResumeId = resume.Id,
                CurrentResumeStep = resume.CurrentStep,
                StreamFullName = _streamService.GetStreamByStudentId(id)?.StreamFullName,
                Summary = _summaryService.GetSummaryByResumeId(resume.Id)?.Text, // fix summarry name. Text was needed.
                ForeignLanguages = _languageService.GetAllForeignLanguagesForResume(resume.Id).ToDictionary(x => x.LanguageId, x => x.LanguageLevelId),
                Educations = _educationService.GetAllEducationsForResume(resume.Id)?.ToArray(),
                Courses = _courseService.GetAllCoursesForResume(resume.Id)?.ToArray(),
                Certificates = _certificateService.GetAllCertificatesForResume(resume.Id)?.ToArray(),
                Exams = _examService.GetAllExamsForResume(resume.Id)?.ToArray(),
                Portfolios = _portfolioService.GetPortfolioForResume(resume.Id)?.ToArray(),
                MilitaryStatus = _militaryStatusService.GetMilitaryStatusForResume(resume.Id),
                Recommendations = _recommendationService.GetAllRecommendationsForResume(resume.Id)?.ToArray(),
                AdditionalInfo = _additionalInfoService.GetAdditionalInfo(resume.Id)
            };

            string[] existingStudentsSkill = _skillService.GetAllSkillsForStudent(id).ToArray();
            if (existingStudentsSkill.Length != 0)
            {
                vm.Skills = _skillService.GetAllSkillsForStudent(id).ToArray();
            }
            else
            {
                var sId = _streamService.GetStreamByStudentId(id)?.Id;
                vm.Skills = _skillService.GetDefaultSkillsByStream((int)sId).ToArray();
            }
            WorkExperience[] workExperiences = _workExperienceService.GetWorkExperienceForResume(resume.Id).ToArray();
            vm.WorkExperiences = new WorkExperienceViewModel[workExperiences.Length];
            if (workExperiences.Count() != 0)
            {
                for (int i = 0; i < workExperiences.Count(); i++)
                {
                    vm.WorkExperiences[i] = new WorkExperienceViewModel(workExperiences[i]);
                }
            }
            return vm;
        }
    }
}
