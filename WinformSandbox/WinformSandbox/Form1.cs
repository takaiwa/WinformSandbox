namespace WinformSandbox
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Data;
    using System.Drawing;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using System.Windows.Forms;

    public partial class Form1 : Form
    {

        private static int maxValue = 10;

        public Form1()
        {
            this.InitializeComponent();
        }

        /// <summary>
        /// 処理開始.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnStart_Click(object sender, EventArgs e)
        {
            this.progressBar1.Visible = true;
            this.progressBar1.Minimum = 1;
            this.progressBar1.Maximum = maxValue;
            this.progressBar1.Value = 1;
            this.progressBar1.Step = 1;

            this.bgWorker.WorkerReportsProgress = true;
            this.bgWorker.WorkerSupportsCancellation = true;

            // 処理開始
            this.bgWorker.RunWorkerAsync(maxValue);
        }

        /// <summary>
        /// 時間のかかる処理をここで行う.
        /// </summary>
        /// <param name="sender">BackgroundWorker.</param>
        /// <param name="e"></param>
        private void BgWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            BackgroundWorker bgWorker = (BackgroundWorker)sender;
            int max = (int)e.Argument;

            Console.WriteLine(string.Format("max:{0}", max));

            for (int i = 1; i < max; i++)
            {
                if (bgWorker.CancellationPending)
                {
                    e.Cancel = true;
                    return;
                }

                // 画面の表示を変更する
                bgWorker.ReportProgress(i);

                // 1000ms待機で疑似重い処理
                System.Threading.Thread.Sleep(1000);
            }

            e.Result = 1;
        }

        /// <summary>
        /// 画面の更新.
        /// ※DoWorkで内では絶対に行わない.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e">進捗が入っている.</param>
        private void BgWorker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            // プログレスバー更新
            this.progressBar1.PerformStep();

            // 進捗を表示
            this.labelProgress.Text = string.Format("{0}%", e.ProgressPercentage.ToString());
        }

        /// <summary>
        /// 処理が終わったら実行される.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e">結果が格納されている.</param>
        private void BgWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (e.Error != null)
            {
                // エラーが発生したとき
                this.labelProgress.Text = "エラー:" + e.Error.Message;
            }
            else if (e.Cancelled)
            {
                // キャンセルされたとき
                this.labelProgress.Text = "キャンセルされました";
            }
            else
            {
                // 正常に終了したとき→結果を表示
                int result = (int)e.Result;
                this.labelProgress.Text = string.Format("結果{0}", result.ToString());
            }
        }

        /// <summary>
        /// キャンセル処理.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnCancel_Click(object sender, EventArgs e)
        {
            this.bgWorker.CancelAsync();
        }
    }
}
