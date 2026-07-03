using Common;
using Common.Utilities;
using Data.Contracts;
using Entities.Medias;
using Microsoft.EntityFrameworkCore;
using Services.Services.MediaServices.DTOs;

namespace Services.Services.MediaServices
{
    public interface IMediaService
    {
        List<GetObjectMediaDTO> GetObjectMedias(long objectId, MediaTypes mediaTypes);
        GetObjectMediaDTO GetObjectMedia(long objectId, MediaTypes mediaTypes);

        Task<List<GetObjectMediaDTO>> GetObjectMediasAsync(long objectId, MediaTypes mediaTypes);
        Task<GetObjectMediaDTO> GetObjectMediaAsync(long objectId, MediaTypes mediaTypes);
    }
    public class MediaService(IRepository<Media> repository) : IMediaService, IScopedDependency
    {
        public List<GetObjectMediaDTO> GetObjectMedias(long objectId, MediaTypes mediaTypes)
        => repository.Table
            .Where(t => t.ObjectId == objectId && t.Type == mediaTypes)
            .Select(t => GetObjectMediaDTO.Create(
                t.Id,
                t.URL,
                t.Type,
                t.Type.ToDisplay(DisplayProperty.Name),
                t.ObjectId
                )
            ).ToList();
        public GetObjectMediaDTO GetObjectMedia(long objectId, MediaTypes mediaTypes)
        => repository.Table
            .Where(t => t.ObjectId == objectId && t.Type == mediaTypes)
            .OrderByDescending(t => t.CreatedAt)
            .Select(t => GetObjectMediaDTO.Create(
                t.Id,
                t.URL,
                t.Type,
                t.Type.ToDisplay(DisplayProperty.Name),
                t.ObjectId
                )
            ).FirstOrDefault();

        public async Task<List<GetObjectMediaDTO>> GetObjectMediasAsync(long objectId, MediaTypes mediaTypes)
        => await repository.Table
            .Where(t => t.ObjectId == objectId && t.Type == mediaTypes)
            .Select(t => GetObjectMediaDTO.Create(
                t.Id,
                t.URL,
                t.Type,
                t.Type.ToDisplay(DisplayProperty.Name),
                t.ObjectId
                )
            ).ToListAsync();

        public async Task<GetObjectMediaDTO> GetObjectMediaAsync(long objectId, MediaTypes mediaTypes)
        => await repository.Table
            .Where(t => t.ObjectId == objectId && t.Type == mediaTypes)
            .OrderByDescending(t => t.CreatedAt)
            .Select(t => GetObjectMediaDTO.Create(
                t.Id,
                t.URL,
                t.Type,
                t.Type.ToDisplay(DisplayProperty.Name),
                t.ObjectId
                )
            ).FirstOrDefaultAsync();
    }
}
