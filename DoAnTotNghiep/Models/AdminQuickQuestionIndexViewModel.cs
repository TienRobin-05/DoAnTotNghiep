namespace DoAnTotNghiep.Models
{
    public class AdminQuickQuestionIndexViewModel
    {
        public int TotalQuestions { get; set; }
        public int VisibleQuestions { get; set; }
        public int HiddenQuestions { get; set; }

        public string? Keyword { get; set; }
        public string? SelectedStatus { get; set; }
        public string? SelectedTopic { get; set; }
        public string? SelectedSort { get; set; }

        public List<string> Topics { get; set; } = new();
        public List<AdminQuickQuestionItemViewModel> Items { get; set; } = new();

        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 5;
        public int TotalItems { get; set; }
        public int StartItem { get; set; }
        public int EndItem { get; set; }
        public int TotalPages { get; set; }
    }

    public class AdminQuickQuestionItemViewModel
    {
        public int Id { get; set; }
        public string QuestionCode { get; set; } = "";
        public string Title { get; set; } = "";
        public string TopicName { get; set; } = "";
        public bool IsVisible { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}
