using AutoMapper;

namespace Core.DotNet.Utilities;

public static class MappingHelper<TSource, TDestination>
{
    public static TDestination Map(TSource request)
    {
        var config = new MapperConfiguration(cfg => cfg.CreateMap<TSource, TDestination>());
        var mapper = config.CreateMapper();
        
        var destination = mapper.Map<TDestination>(request);

        return destination;
    }
}