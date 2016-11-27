namespace Kastil.Core
{
    public static class Messages
    {
        public static class General
        {
            public const string Loading = "Loading";
			public const string Syncing = "Syncing";
            public const string Saving = "Saving";
            public const string AssessmentSaved = "Assessment saved successfully";
            public const string ShelterSaved = "Shelter saved successfully";
            public const string SavedSuccessfully = "saved successfully";
            public const string SynchronizeData = "Synchronize data?";
            public const string SomethingWentWrongPleaseTryAgain = "Something went wrong. Please try again.";
            public const string DefaultNewAssessmentName = "New Assessment";
            public const string DefaultNewShelterName = "New Shelter";
            public const string Unknown = "Unknown";
        }

		public static class Placeholders
		{
			public const string AssessmentName = "Enter assessment name";
			public const string AssessmentLocation = "Enter assessment location";
            public const string ShelterName = "Enter shelter name";
            public const string ShelterLocation = "Enter shelter location";
        }

        public static class Login
        {
            public const string EmailIsRequired = "Email is required";
            public const string PasswordIsRequired = "Password is required";
            public const string LoggingYouIn = "Logging you in";
            public const string LoggingYouOut = "Logging you out ...";
        }


        public static class DisasterMenu
        {
            public const string Assessment = "Assessment";
            public const string Shelters = "Shelters";
        }
    }
}
