using System;
using System.Collections.Generic;
using System.Text;
using Gapi.Language;
using System.Windows.Forms;

namespace Translator
{
    public delegate void TranslateThreadEvent(string result);
    public delegate void TranslateThreadError(Exception error);
   public class TranslateThread
    {
       public event TranslateThreadEvent TranslateComplete;
       public event TranslateThreadError TranslationError;
       public Control Container { get; set; }
       public string Text { get; set; }
       public Language SourceLanguage { get; set; }
       public Language targetLanguage { get; set; }
       private System.Threading.Thread thread;
       public TranslateThread() { }
       public TranslateThread(string Text, Language SourceLanguage, Language targetLanguage)
       {
           SetInputs(Text, SourceLanguage, targetLanguage);
       }

       public void SetInputs(string Text, Language SourceLanguage, Language targetLanguage)
       {
           this.Text = Text;
           this.targetLanguage = targetLanguage;
           this.SourceLanguage = SourceLanguage;
       }
       private void ThreadMethod()
       {
           try
           {
               
               string r =Gapi.Language. Translator.Translate(Text, SourceLanguage, targetLanguage);
               if (TranslateComplete == null) return;
               if (Container == null)
                   TranslateComplete(r);
               else Container.Invoke(new TranslateThreadEvent(TranslateComplete), r);
           }
           catch (Exception error)
           {
               if (TranslationError == null) return;
               if (Container == null) TranslationError(error);
               else Container.Invoke(new TranslateThreadError(TranslationError), error);
           }
           finally {
               if (thread != null)
               {
                   thread.Abort();

                   thread = null;
               }
           }
       }

       public void Start()
       {
           try
           {
               if (thread != null) Aboart();
               thread = new System.Threading.Thread(ThreadMethod);
               thread.Start();
           }
           catch
           {
               thread = null;

           }
           
       }

       public void Aboart()
       {
           try
           {
               Gapi.Core.CoreHelper.request.Abort();
               thread.Abort();
              
           }
           catch { }
           finally
           {
               thread = null;
           }
       }
    }
}
