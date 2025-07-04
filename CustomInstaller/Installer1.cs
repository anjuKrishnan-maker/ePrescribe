using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration.Install;
using System.Linq;
using System.Diagnostics;

namespace CustomInstaller
{
    [RunInstaller(true)]
    public partial class Installer1 : System.Configuration.Install.Installer
    {
        public Installer1()
        {
            InitializeComponent();
        }

        public override void Install(System.Collections.IDictionary stateSaver)
        {
            base.Install(stateSaver);

            // uncomment this and run with the DEBUG configuration manager setting to step thru in debugger
            //System.Diagnostics.Debugger.Launch();

            int environment1;
            int environment2;
            string targetDir;

            if (Context.Parameters["Environment1"] == null)
            {
                //
                // TODO: log in event log
                //
                //return;
                throw new Exception("No value selected from first environment dialog screen.");
            }
            else
            {
                if (!Int32.TryParse(Context.Parameters["Environment1"], out environment1))
                {
                    //
                    // TODO: log in event log
                    //
                    //return;
                    throw new Exception("Could not covert Environment1 param to integer.");
                }
            }

            if (Context.Parameters["Environment2"] == null)
            {
                //
                // TODO: log in event log
                //
                //return;
                throw new Exception("No value selected from second environment dialog screen.");
            }
            else
            {
                if (!Int32.TryParse(Context.Parameters["Environment2"], out environment2))
                {
                    //
                    // TODO: log in event log
                    //
                    //return;
                    throw new Exception("Could not covert Environment1 param to integer.");
                }
            }

            if (environment1 == 4 && environment2 == 3)
            {
                // both environments are None
                //
                // TODO: log in event log
                //
                //return;
                throw new Exception("No environments have been selected.");
            }

            if (environment1 < 4 && environment2 < 3)
            {
                // environments have been selected for both panels.
                //
                // TODO: log in event log
                //
                //return;
                throw new Exception("More than 1 environment has been selected.");
            }

            if (Context.Parameters["TargetDir"] == null)
            {
                //
                // TODO: log in event log
                //
                //return;
                throw new Exception("No value found for target directory.");
            }
            else
            {
                targetDir = Context.Parameters["TargetDir"].ToString();
            }

            try
            {
                switch (environment1)
                {
                    case 1:
                        // set the Production config as web.config
                        File.Copy(targetDir + "web.prod.config", targetDir + "web.config", true);
                        break;
                    case 2:
                        // set the INT config as web.config
                        File.Copy(targetDir + "web.int.config", targetDir + "web.config", true);
                        break;
                    case 3:
                        // set the QA config as web.config
                        File.Copy(targetDir + "web.qa.config", targetDir + "web.config", true);
                        break;
                    case 4:
                        // None has been selected on the first environment dialong box
                        // so check the value of the second environment dialog box

                        switch (environment2)
                        {
                            case 1:
                                // set the Staging config as web.config
                                File.Copy(targetDir + "web.stage.config", targetDir + "web.config", true);
                                break;
                            case 2:
                                // set the DR config as web.config
                                File.Copy(targetDir + "web.dr.config", targetDir + "web.config", true);
                                break;
                        }
                        break;
                }
            }
            catch (Exception ex)
            {
                //
                // TODO: log in event log
                //
                throw new Exception("Error copying config file: " + ex.Message);
            }
        }

        public override void Commit(IDictionary savedState)
        {
            base.Commit(savedState);
        }

        public override void Rollback(IDictionary savedState)
        {
            base.Rollback(savedState);
        }

        public override void Uninstall(IDictionary savedState)
        {
            base.Uninstall(savedState);
        }

    }
}
