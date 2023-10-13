using System;
using System.IO;
using System.Text;
using System.Linq;
using System.Diagnostics;
using Autodesk.Revit.UI;
using Autodesk.Revit.DB;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB.Structure;
using System.Windows.Forms;
using System.Threading.Tasks;
using System.Collections.Generic;
using BIMAPI_HW2;

namespace APIFinal
{
	[Autodesk.Revit.Attributes.Transaction(Autodesk.Revit.Attributes.TransactionMode.Manual)]

	public class _0104_test : IExternalCommand
	{
		private class ObjectType : EqualityComparer<ObjectType>
		{
			int[] _dim = new int[2];

			public int H
			{
				get { return _dim[0]; }
			}

			public int W
			{
				get { return _dim[1]; }
			}

			public string Name
			{
				get { return H.ToString() + " x " + W.ToString() + "mm"; }
			}

			public ObjectType(int d1, int d2)
			{
				if (d1 > d2)
				{
					_dim = new int[] { d1, d2 };
				}
				else
				{
					_dim = new int[] { d2, d1 };
				}
			}

			public override bool Equals(ObjectType x, ObjectType y)
			{
				return x.H == y.H && x.W == y.W;
			}

			public override int GetHashCode(ObjectType obj)
			{
				return obj.Name.GetHashCode();
			}
		}

		public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
		{

			void get_number_of_Object(string path, ref int num)
			{
				StreamReader reader = new StreamReader(path);
				while (reader.Peek() >= 0)
				{
					string line = reader.ReadLine();
					num++;
				}
				num--;
				reader.Close();
			}


			void get_ObjectType(string path, ref int[] L1, ref int[] L2)
			{
				StreamReader reader = new StreamReader(path);
				int count = 0;
				while (reader.Peek() >= 0)
				{
					string line = reader.ReadLine();
					if (line == "值") { continue; }
					string[] input = line.Split('X');
					L1[count] = Int32.Parse(input[0]);
					L2[count] = Int32.Parse(input[1]);
					count++;
				}
				reader.Close();
			}

			//for loading the BIM model
			void load_rfa_element(ref Document document, string path, string name)
			{
				Family family = null;
				using (Transaction tx = new Transaction(document))
				{
					try
					{
						tx.Start("Load Object");
						//load family
						document.LoadFamily(path, out family);
						tx.Commit();
						MessageBox.Show("Element: " + name + " has been add");
					}
					catch (Exception e)
					{
						//create dialog box for error
						TaskDialog td = new TaskDialog("Error");

						td.Title = "Error";
						td.AllowCancellation = true;

						//message related stuffs
						td.MainInstruction = "Error";
						td.MainContent = "Error: " + e.Message;

						//common button stuffs
						td.CommonButtons = TaskDialogCommonButtons.Close;

						//dialog showup stuffs
						td.Show();

						Debug.Print(e.Message);
						tx.RollBack();
					}
				}
			}

			//main
			UIDocument uidoc = commandData.Application.ActiveUIDocument;
			Autodesk.Revit.UI.Selection.Selection selElement = uidoc.Selection;
			Autodesk.Revit.DB.Document doc = uidoc.Document;

			Application.EnableVisualStyles();
			Form1 form = new Form1(doc, selElement);
			Application.Run(form);
			// 載入族群
			string rfa_path = form.path1;
			string rfa_name = form.filename.Split('.')[0];
			// 參數資料
			string Object_type_path = form.path2;
			string Object = form.Object;
			// MessageBox.Show("rfa_path:" + rfa_path + " rfa_name" + rfa_name + " Object_type_path:" + Object_type_path + " Object:" + Object);
			int col_num = 0;
            try
            {
				get_number_of_Object(Object_type_path, ref col_num);
				int[] length1 = new int[col_num];
				int[] length2 = new int[col_num];
				get_ObjectType(Object_type_path, ref length1, ref length2);

				List<ObjectType> all = new List<ObjectType>();
				for (int i = 0; i < col_num; ++i)
					all.Add(new ObjectType(length1[i], length2[i]));
				all = all.Distinct(new ObjectType(0, 0)).ToList();


				load_rfa_element(ref doc, rfa_path, rfa_name);

				//change Object's type
				//Beam:BuiltInCategory.OST_StructuralFraming;Column:BuiltInCategory.OST_StructuralColumns
				FilteredElementCollector symbols = null;
				if (Object == "Beam")
				{
					symbols = new FilteredElementCollector(doc)
				.WhereElementIsElementType().OfCategory(BuiltInCategory.OST_StructuralFraming);
				}
				else if (Object == "Column")
				{
					symbols = new FilteredElementCollector(doc)
				.WhereElementIsElementType().OfCategory(BuiltInCategory.OST_StructuralColumns);
				}

				IEnumerable<FamilySymbol> existing
				= symbols.Cast<FamilySymbol>().Where<FamilySymbol>(x => x.FamilyName.Equals(rfa_name));

				if (0 == existing.Count())
				{
					return Result.Cancelled;
				}

				List<ObjectType> AlreadyExists = new List<ObjectType>();
				List<ObjectType> ToBeMade = new List<ObjectType>();

				for (int i = 0; i < all.Count; ++i)
				{
					FamilySymbol fs = existing.FirstOrDefault(x => x.Name == all[i].Name);

					if (fs == null)
					{
						ToBeMade.Add(all[i]);
					}
					else
					{
						AlreadyExists.Add(all[i]);
					}
				}

				if (ToBeMade.Count == 0)
				{
					return Result.Cancelled;
				}
				//MessageBox.Show("count= " + ToBeMade.Count);
				using (Transaction tx = new Transaction(doc))
				{
					if (tx.Start("Make types") == TransactionStatus.Started)
					{
						FamilySymbol first = existing.First();
						foreach (ObjectType ct in ToBeMade)
						{
							ElementType et = first.Duplicate(ct.Name);
							// Use actual type parameter names
							// Use GUIDs instead of LookupParameter where possible
							try
							{
								et.LookupParameter("柱深").Set(ct.H / 304.8);
								et.LookupParameter("柱寬").Set(ct.W / 304.8);
							}
							catch (System.NullReferenceException)
							{
								et.LookupParameter("h").Set(ct.H / 304.8);
								et.LookupParameter("b").Set(ct.W / 304.8);
							}
							catch (Exception e)
							{
								//create dialog box for error
								TaskDialog td = new TaskDialog("Error");

								td.Title = "Error";
								td.AllowCancellation = true;

								//message related stuffs
								td.MainInstruction = "Error";
								td.MainContent = "Error: " + e.Message;

								//common button stuffs
								td.CommonButtons = TaskDialogCommonButtons.Close;

								//dialog showup stuffs
								td.Show();

								Debug.Print(e.Message);
								tx.RollBack();
							}
						}
						tx.Commit();
					}
					string ObjectMessageBox = "";
					for (int i = 0; i < col_num; i++)
					{
						ObjectMessageBox += "Create " + Object + " " + length1[i] + " X " + length2[i] + " mm\n";
					}
					MessageBox.Show(ObjectMessageBox);
				}
				MessageBox.Show("Create " + Object + "type Succeeded");
				return Result.Succeeded;
			}

			catch(Exception e)
            {
				Console.WriteLine(e);
            }
			return Result.Failed;
		}
	}
}


/*
string Object = "Beam";//結構樑or結構柱
string rfa_name = "混凝土-矩形樑";
string rfa_path = "C:/ProgramData/Autodesk/RVT 2019/Libraries/China_Trad/結構構架/混凝土/混凝土-矩形樑.rfa";
string Object_type_path = "C:/Users/jim/Desktop/test.txt";
*/
