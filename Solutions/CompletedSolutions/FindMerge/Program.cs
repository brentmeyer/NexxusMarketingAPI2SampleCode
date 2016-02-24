//===================================================================
//=																	=
//= Copyright (c) 2016 IMS Health Incorporated. All rights reserved.=
//=																	=
//===================================================================

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace FindMerge
{
    class Program
    {
        static void Main(string[] args)
        {
            string email = "<EMAIL ADDRESS>";
            
            Merge merge = new Merge();
            try {
                merge.ProcessFindMerge(email);
            }
            catch (NexxusException e) {

                switch (e.NexxusErrorString) {
                    case NexxusErrorCodes.IdsNotFound:
                        Console.WriteLine("No contacts found for email: {0}.", email);
                        break;
                    case NexxusErrorCodes.InvalidCredentials:
                        Console.WriteLine("Invalid credentials. Check AppConfigFile.config.");
                        break;
                    default:
                        Console.WriteLine(e.NexxusErrorString + ": " + e.ErrorMessage);
                        break;
                }
            }
            catch (Exception e) {
                Console.WriteLine("Merge Operation failed:", e.Message);
            }
            Console.WriteLine("Done.");
            Console.Read();
       }
    }
 }
