using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

/// <summary>
/// Summary description for PatientMedAndAllergyRecEvent
/// </summary>

namespace eRxWeb
{
    public class PatientMedAndAllergyRecEventArgs
    {

        public enum ActionType
        {
            SAVE = 0,
            SAVE_PRESCRIBE,
        }

        private ActionType _Action;
        private string _Message;

        public PatientMedAndAllergyRecEventArgs(string msg, ActionType A)
        {
            _Message = msg;
            _Action = A;
        }

        public string Message
        {
            get { return _Message; }
            set { _Message = value; }
        }

        public ActionType Action
        {
            get { return _Action; }
            set { _Action = value; }
        }

    }


    public delegate void PatientMedAndAllergyRecEventHandler(object sender, PatientMedAndAllergyRecEventArgs e);
}