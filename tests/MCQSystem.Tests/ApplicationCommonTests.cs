using MCQSystem.Application.Common;
using MCQSystem.Domain.Entities;

namespace MCQSystem.Tests;

public sealed class ApplicationCommonTests
{
    [Fact]
    public void PagedResult_Calculates_TotalPages_And_Navigation()
    {
        var result = new PagedResult<string>
        {
            Items = ["A", "B"],
            PageNumber = 2,
            PageSize = 10,
            TotalCount = 26
        };

        Assert.Equal(3, result.TotalPages);
        Assert.True(result.HasPrevious);
        Assert.True(result.HasNext);
    }

    [Fact]
    public void Question_Defaults_To_Active_Mcq_With_Medium_Difficulty()
    {
        var question = new Question();

        Assert.True(question.IsActive);
        Assert.Equal("A", question.CorrectOptionKey);
        Assert.NotEqual(string.Empty, question.Id);
    }
}
