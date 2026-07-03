
using AutoMapper;
using Entities.Products;
using WebFramework.Api;

namespace PadyLife.Api.Models.DTOs
{
    public class ProductCategoryDTO : BaseDto<ProductCategoryDTO, ProductCategory, long>
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public long? ParentCategoryId { get; set; }
        public string ParentName { get; set; }
        public string ImageUrl { get; set; }
        public List<ProductCategoryDTO> ChildCategories { get; set; }
        public List<ProductCategoryAttributesDTO> ProductCategoryAttributes { get; set; }

        public override void CustomMappings(IMappingExpression<ProductCategory, ProductCategoryDTO> mapping)
        {
            mapping.ForMember(
                dest => dest.ChildCategories,
                config => config.MapFrom(src => src.ChildCategories.Select(pc => new ProductCategoryDTO
                {
                    Name = pc.Name,
                    Description = pc.Description,
                    ParentCategoryId = pc.ParentCategoryId,
                    ParentName = pc.ParentCategory.Name,

                })));

            mapping.ForMember(
            dest => dest.ProductCategoryAttributes,
            config => config.MapFrom(src => src.Attributes.Select(pc => new ProductCategoryAttributesDTO
            {
                Name = pc.Attribute.Name,
                AttributeId = pc.AttributeId,
                CategoryId = pc.CategoryId,
                CategoryName = pc.Category.Name,
                IsRequired = pc.IsRequired,
                IsVariant = pc.IsVariant
            })));
        }
        public class ProductCategoryAttributesDTO
        {
            public string Name { get; internal set; }
            public long AttributeId { get; internal set; }
            public long CategoryId { get; internal set; }
            public string CategoryName { get; internal set; }
            public bool IsRequired { get; internal set; }
            public bool IsVariant { get; internal set; }
        }
    }
}
