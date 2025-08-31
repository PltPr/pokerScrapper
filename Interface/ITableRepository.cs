using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using api.Dto;

namespace api.Interface
{
    public interface ITableRepository
    {
        Task<CaptureDto> CaptureTable();
        string[] GetHand();
    }
}