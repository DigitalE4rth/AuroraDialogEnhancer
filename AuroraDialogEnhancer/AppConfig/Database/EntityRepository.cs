using System;
using System.IO;
using System.Xml.Serialization;
using AuroraDialogEnhancer.Frontend.Forms.Utils;

namespace AuroraDialogEnhancer.AppConfig.Database;

public abstract class EntityRepository<T> where T : class, new()
{
    public void Save(T entity, string path)
    {
        try
        {
            var serializer = XmlSerializer.FromTypes(new[] { typeof(T) })[0];
            using var fileStream = new FileStream(path, FileMode.Create);
            serializer.Serialize(fileStream, entity);
        }
        catch (Exception e)
        {
            new InfoDialogBuilder()
                .SetWindowTitle(Properties.Localization.Resources.EntityRepository_Error_Read)
                .SetMessage(e.Message + Environment.NewLine + e.InnerException?.Message)
                .SetTypeError()
                .ShowDialog();
        }
    }

    public T Get(string path)
    {
        try
        {
            var serializer = XmlSerializer.FromTypes(new[] { typeof(T) })[0];
            using var fileStream = new FileStream(path, FileMode.Open);
            return (T) serializer.Deserialize(fileStream);
        }
        catch (Exception e)
        {
            new InfoDialogBuilder()
                .SetWindowTitle(Properties.Localization.Resources.EntityRepository_Error_Write)
                .SetMessage(e.Message + Environment.NewLine + e.InnerException?.Message)
                .SetTypeError()
                .ShowDialog();
        }

        return new T();
    }
}
