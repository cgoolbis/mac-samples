﻿using System;
using System.Collections.Generic;
using System.Linq;
using Foundation;
using AppKit;

namespace MacDatabinding
{
	public partial class SubviewTableBindingController : AppKit.NSViewController
	{
		#region Private Variables
		private NSMutableArray _people = new NSMutableArray();
		#endregion

		#region Computed Properties
		//strongly typed view accessor
		public new SubviewTableBinding View {
			get {
				return (SubviewTableBinding)base.View;
			}
		}

		[Export("personModelArray")]
		public NSArray People {
			get { return _people; }
		}

		public PersonModel SelectedPerson { get; private set; }
		#endregion

		#region Constructors
		// Called when created from unmanaged code
		public SubviewTableBindingController (IntPtr handle) : base (handle)
		{
			Initialize ();
		}

		// Called when created directly from a XIB file
		[Export ("initWithCoder:")]
		public SubviewTableBindingController (NSCoder coder) : base (coder)
		{
			Initialize ();
		}

		// Call to load from the XIB/NIB file
		public SubviewTableBindingController () : base ("SubviewTableBinding", NSBundle.MainBundle)
		{
			Initialize ();
		}

		// Shared initialization code
		void Initialize ()
		{
		}
		#endregion

		#region Public Methods
		public void DeletePerson(NSWindow window) {
			if (View.SelectedRow == -1) {
				var alert = new NSAlert () {
					AlertStyle = NSAlertStyle.Critical,
					InformativeText = "Please select the person to remove from the list of people.",
					MessageText = "Delete Person",
				};
				alert.BeginSheet (window);
			} else {
				// Grab person
				SelectedPerson = _people.GetItem<PersonModel> ((nuint)View.SelectedRow);

				// Confirm delete
				var alert = new NSAlert () {
					AlertStyle = NSAlertStyle.Critical,
					InformativeText = string.Format("Are you sure you want to delete person `{0}` from the table?",SelectedPerson.Name),
					MessageText = "Delete Person",
				};
				alert.AddButton ("Ok");
				alert.AddButton ("Cancel");
				alert.BeginSheetForResponse (window, (result) => {
					// Delete?
					if (result == 1000) {
						RemovePerson(View.SelectedRow);
					}
				});
			}
		}

		public void EditPerson(NSWindow window) {
			if (View.SelectedRow == -1) {
				var alert = new NSAlert () {
					AlertStyle = NSAlertStyle.Informational,
					InformativeText = "Please select the person to edit from the list of people.",
					MessageText = "Edit Person",
				};
				alert.BeginSheet (window);
			} else {
				// Grab person
				SelectedPerson = _people.GetItem<PersonModel> ((nuint)View.SelectedRow);

				var sheet = new PersonEditorSheetController(SelectedPerson, false);

				// Display sheet
				sheet.ShowSheet(window);

			}
		}

		public void FindPerson(string text) {

			// Convert to lower case
			text = text.ToLower ();

			// Scan each person in the list
			for (nuint n = 0; n < _people.Count; ++n) {
				var person = _people.GetItem<PersonModel> (n);
				if (person.Name.ToLower ().Contains (text)) {
					View.Table.SelectRow ((nint)n, false);
					return;
				}
			}

			// Not found, select none
			View.Table.DeselectAll (this);
		}
		#endregion

		#region Override Methods
		public override void AwakeFromNib ()
		{
			base.AwakeFromNib ();

			// Build list of employees
			AddPerson (new PersonModel ("Craig Dunn", "Documentation Manager", true));
			AddPerson (new PersonModel ("Amy Burns", "Technical Writer"));
			AddPerson (new PersonModel ("Joel Martinez", "Web & Infrastructure"));
			AddPerson (new PersonModel ("Kevin Mullins", "Technical Writer"));
			AddPerson (new PersonModel ("Mark McLemore", "Technical Writer"));
			AddPerson (new PersonModel ("Tom Opgenorth", "Technical Writer"));
			AddPerson (new PersonModel ("Larry O'Brien", "API Documentation Manager", true));
			AddPerson (new PersonModel ("Mike Norman", "API Documentor"));

		}
		#endregion

		#region Array Controller Methods
		[Export("addObject:")]
		public void AddPerson(PersonModel person) {
			WillChangeValue ("personModelArray");
			_people.Add (person);
			DidChangeValue ("personModelArray");
		}

		[Export("insertObject:inPersonModelArrayAtIndex:")]
		public void InsertPerson(PersonModel person, nint index) {
			WillChangeValue ("personModelArray");
			_people.Insert (person, index);
			DidChangeValue ("personModelArray");
		}

		[Export("removeObjectFromPersonModelArrayAtIndex:")]
		public void RemovePerson(nint index) {
			WillChangeValue ("personModelArray");
			_people.RemoveObject (index);
			DidChangeValue ("personModelArray");
		}

		[Export("setPersonModelArray:")]
		public void SetPeople(NSMutableArray array) {
			WillChangeValue ("personModelArray");
			_people = array;
			DidChangeValue ("personModelArray");
		}
		#endregion

	}
}
