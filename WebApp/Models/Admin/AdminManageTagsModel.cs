using Domain.Entities;

namespace WebApp.Models.Admin
{
    public class AdminManageTagsModel
    {
        public class TagData
        {
            public string TagId { get; set; } = null!;
            public string? TagName { get; set; } = null!;
        }

        public AdminManageTagsModel(IList<Tag> tags)
        {
            for (int i = 0; i < tags.Count; i++)
            {
                TagData tagData = new()
                {
                    TagName = tags[i].TagName,
                    TagId = tags[i].Id
                };

                Tags.Add(tagData);
            }
        }

        public List<TagData> Tags { get; set; } = new List<TagData>();
    }
}