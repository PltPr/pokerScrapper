using api.Dto;

namespace api.Interface
{
    public interface ITableRepository
    {
        Task<CaptureDto> CaptureTable();
        string[] GetHand();
        string[] GetTableCards();

    }
}