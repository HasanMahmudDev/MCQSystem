using AutoMapper;
using MCQSystem.Application.DTOs;
using MCQSystem.Application.Interfaces;
using MCQSystem.Domain.Entities;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace MCQSystem.WebUI.Pages.Student.Exams;

public sealed class ResultModel(IRepository<Result> results, IMapper mapper) : PageModel
{
    public ResultDto? Result { get; private set; }

    public async Task OnGetAsync(string resultId, CancellationToken cancellationToken)
    {
        var result = await results.GetByIdAsync(resultId, cancellationToken);
        if (result is not null)
        {
            Result = mapper.Map<ResultDto>(result);
        }
    }
}
