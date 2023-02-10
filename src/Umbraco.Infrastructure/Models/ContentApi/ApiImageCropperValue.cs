using Umbraco.Cms.Core.PropertyEditors.ValueConverters;

namespace Umbraco.Cms.Core.Models.ContentApi;

public class ApiImageCropperValue
{
    public ApiImageCropperValue(string url, ImageCropperValue.ImageCropperFocalPoint? focalPoint, IEnumerable<ImageCropperValue.ImageCropperCrop>? crops)
    {
        Url = url;
        FocalPoint = focalPoint;
        Crops = crops;
    }

    public string Url { get; }

    public ImageCropperValue.ImageCropperFocalPoint? FocalPoint { get; }

    public IEnumerable<ImageCropperValue.ImageCropperCrop>? Crops { get; }
}