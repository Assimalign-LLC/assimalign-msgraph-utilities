using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Assimalign.OneDrive.Tooler.Options
{

	public enum StructureType
	{
		Parent = 1,
		Child = 2
	}

	public class DriveStructure
	{
		private DriveStructure Current { get; set; }
		private string Parent { get; set; }

		public StructureType StructureType { get; set; }

		public string Name { get; set; }

		public IEnumerable<DriveStructure> Children { get; set; }


		public Dictionary<string, string> GetFolderCollection()
		{
			var dictionary = new Dictionary<string, string>();
			Current = this;

			SelfLoop<DriveStructure>(Current, action =>
			{
				if (Current.Children != null)
				{
					foreach (var child in Current.Children)
					{
						if (Current.Parent == null)
							child.Parent = Current.Name;

						else
							child.Parent = Current.Parent + "/" + Current.Name;
					}
				}

				if (Current.Parent == null)
					dictionary.Add(Current.Name, Current.Name);

				else
					dictionary.Add(Current.Name, Current.Parent);
			});

			return dictionary;
		}

		private void SelfLoop<TObject>(TObject customType, Action<TObject> action) where TObject : class, new()
		{
			var properties = typeof(TObject).GetProperties();
			var finished = false;

			foreach (var property in properties)
			{
				if (property.PropertyType == typeof(IEnumerable<TObject>))
				{
					var currentValue = (IEnumerable<TObject>)property.GetValue(customType);

					if (null != currentValue)
					{
						foreach (var childObject in currentValue)
						{
							Current = childObject as DriveStructure;
							SelfLoop<TObject>(childObject, action);
						}
					}
				}
				else if (!finished)
				{
					action(customType);
					finished = true;
				}
			}
		}
	}
}
