using Luxand;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace VimassFVA
{
    public partial class Register : Form
    {
        bool needClose = false;
        String cameraName;

        FSDK.TFacePosition facePosition_Global;
        bool isLiveness = false;
        byte[] template_Global;
        FSDK.CImage image_Global;
        static byte[] template_FromFile;
        public Register(string user)
        {
            InitializeComponent();
            this.label1.Text = "Xin chào: " + user;
            Ulti.soVi = user;
            initCamera();
        }
        
        public void initCamera()
        {
            String keyLX = "uypkToItYK8NviZCLG+n9L6lgekJ5n5TWWkroruVGQf+Ku3pl30qunu" +
                "TAYchwRC2MLKLubUspp+QI4BTUNHnCSiZbEcHmoOmxE4e/HTHik6bxM7I5V9LnggPE" +
                "yDw8ga1Q4IfbBE5aR4mc9RgKRz6e9y/99phHELlXK03W1zur6w=";
            if (FSDK.FSDKE_OK != FSDK.ActivateLibrary(keyLX))
            {
                MessageBox.Show("License Key Hết Hạn", "Lỗi rồi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                Application.Exit();
            }

            FSDK.InitializeLibrary();
            FSDKCam.InitializeCapturing();

            string[] cameraList;
            int count;
            FSDKCam.GetCameraList(out cameraList, out count);

            if (0 == count)
            {
                MessageBox.Show("Vui lòng cho phép sử dụng camera", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                Application.Exit();
            }
            cameraName = cameraList[0];

            FSDKCam.VideoFormatInfo[] formatList;
            FSDKCam.GetVideoFormatList(ref cameraName, out formatList, out count);            
            int VideoFormat = 0; // Chọn một định dạng video
            pictureBox1.Width = formatList[VideoFormat].Width;
            pictureBox1.Height = formatList[VideoFormat].Height;

            taoThreadMoi();            
        }
        Thread thread;
        public void taoThreadMoi()
        {
            thread = new Thread(delegate ()
            {
                khoiDongCamera();
            });
            thread.Start();
        }


        private void khoiDongCamera()
        {           
            int cameraHandle = 0;
            Debug.WriteLine("Khoi tao camera");
            int r = FSDKCam.OpenVideoCamera(ref cameraName, ref cameraHandle);
            if (r != FSDK.FSDKE_OK)
            {
                MessageBox.Show("Lỗi mở camera thứ 1", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                Application.Exit();
            }

            int tracker = 0; 	// creating a Tracker
            FSDK.CreateTracker(ref tracker); // if could not be loaded, create a new tracker

            int err = 0; // set realtime face detection parameters
            FSDK.SetTrackerMultipleParameters(tracker, "HandleArbitraryRotations=false; DetermineFaceRotationAngle=false; InternalResizeWidth=100; FaceDetectionThreshold=5;", ref err);
            FSDK.SetTrackerParameter(tracker, "DetectLiveness", "true"); // enable liveness
            FSDK.SetTrackerParameter(tracker, "SmoothAttributeLiveness", "true"); // use smooth minimum function for liveness values
            FSDK.SetTrackerParameter(tracker, "AttributeLivenessSmoothingAlpha", "1"); // smooth minimum parameter, 0 -> mean, inf -> min
            FSDK.SetTrackerParameter(tracker, "LivenessFramesCount", "15"); // minimal number of frames required to output liveness attribute

            while (!needClose)
            {                
                Int32 imageHandle = 0;
                if (FSDK.FSDKE_OK != FSDKCam.GrabFrame(cameraHandle, ref imageHandle)) // grab the current frame from the camera
                {
                    
                    Application.DoEvents();
                    continue;
                }

                
                image_Global = new FSDK.CImage(imageHandle);

                long[] IDs;
                long faceCount = 0;
                FSDK.FeedFrame(tracker, 0, image_Global.ImageHandle, ref faceCount, out IDs, sizeof(long) * 256); // maximum of 256 faces detected
                Array.Resize(ref IDs, (int)faceCount);

                // make UI controls accessible (to find if the user clicked on a face)
                Application.DoEvents();

                Image frameImage = image_Global.ToCLRImage();
                Graphics gr = Graphics.FromImage(frameImage);

                for (int i = 0; i < IDs.Length; ++i)
                {
                    FSDK.TFacePosition facePosition = new FSDK.TFacePosition();
                    FSDK.GetTrackerFacePosition(tracker, 0, IDs[i], ref facePosition);

                    int left = facePosition.xc - (int)(facePosition.w * 0.6);
                    int top = facePosition.yc - (int)(facePosition.w * 0.5);
                    int w = (int)(facePosition.w * 1.2);

                   String statusText;
                    StringFormat format = new StringFormat();
                    format.Alignment = StringAlignment.Center;
                    Brush brush;
                    Pen pen;
                    string value;
                    float liveness = 0;

                    int res = FSDK.GetTrackerFacialAttribute(tracker, cameraHandle, IDs[i], "Liveness", out value, 1024);
                    if (res == FSDK.FSDKE_OK)
                    {
                        res = FSDK.GetValueConfidence(value, "Liveness", ref liveness);
                        if (liveness > 0.5f)
                        {
                            isLiveness = true;                           
                        }
                    }

                    if (res != FSDK.FSDKE_OK)
                    {
                        pen = Pens.LightGreen;
                        brush = new System.Drawing.SolidBrush(System.Drawing.Color.LightGreen);
                        statusText = "";
                    }
                    else if (liveness > 0.5f)
                    {
                        isLiveness = true;
                        pen = Pens.LightGreen;
                        brush = new System.Drawing.SolidBrush(System.Drawing.Color.LightGreen);
                        statusText = "\"Vui lòng lựa chọn hành động\"";
                    }
                    else
                    {
                        pen = Pens.Red;
                        brush = new System.Drawing.SolidBrush(System.Drawing.Color.Red);
                        statusText = "\"Đây không phải người thật\"";
                    }

                    gr.DrawString(statusText, new System.Drawing.Font("Arial", 16),
                        brush, facePosition.xc, top + w + 5, format);
                    gr.DrawRectangle(pen, left, top, w, w);
                }                
                // display current frame
                pictureBox1.Image = frameImage;
                GC.Collect(); // collect the garbage after the deletion               
            }

            FSDK.FreeTracker(tracker);
            FSDKCam.CloseVideoCamera(cameraHandle);
            FSDKCam.FinalizeCapturing();
            
        }

        private void dangKy()
        {
            int res2 = FSDK.DetectFace(image_Global.ImageHandle, ref facePosition_Global);
            Debug.WriteLine("res2 " + res2);
            if (res2 == FSDK.FSDKE_FACE_NOT_FOUND)
            {
                /*lb_thongBao.Text = "\"Không tìm thấy khuôn mặt\"";
                lb_thongBao.ForeColor = System.Drawing.Color.Red;*/
            }
            else if (res2 == FSDK.FSDKE_IMAGE_TOO_SMALL)
            {
                /*lb_thongBao.Text = "\"Vui lòng di chuyển gần camera hơn\"";
                lb_thongBao.ForeColor = System.Drawing.Color.Red;*/
            }
            else if (res2 == FSDK.FSDKE_OK)
            {
                if (isLiveness)
                {
                    template_Global = new byte[FSDK.TemplateSize];
                    FSDK.GetFaceTemplate(image_Global.ImageHandle, out template_Global);
                    try
                    {                        
                        luuThongTin(Ulti.path, template_Global, Ulti.pathSoVi, Ulti.soVi);
                        template_FromFile = template_Global;                                             
                        MessageBox.Show("Đăng kí khuôn mặt thành công", "Thông tin", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        needClose = true;
                        thread.Abort();
                        // goi dich vu thong bao cho web
                        // chuyen sang authen;                        
                        new Authen(Ulti.soVi).Show();
                        this.Dispose();
                    }
                    catch (Exception)
                    {
                        /* lb_thongBao.Text = "\"Đăng kí khuôn mặt không thành công thành công" +
                               "\r\nLỗi khi lưu thông tin\"";
                         lb_thongBao.ForeColor = System.Drawing.Color.Red;*/
                        Debug.WriteLine("Đăng kí khuôn mặt không thành công thành công!");
                    }
                }
            }
        }

        private void luuThongTin(string path, byte[] template_Global, string pathSoVi, string soVi)
        {
            string soVi_MaHoa = MaHoaDuLieu.maHoa(soVi);
            File.WriteAllText(pathSoVi, soVi_MaHoa);

            string temp = Encoding.ASCII.GetString(template_Global);
            string template_Global_MaHoa = MaHoaDuLieu.maHoa(temp, soVi);
            File.WriteAllText(path, template_Global_MaHoa);
        }

        private void btnReg_Click(object sender, EventArgs e)
        {
            dangKy();
        }

        private void Register_FormClosing(object sender, FormClosingEventArgs e)
        {
            needClose = true;
            this.Dispose();            
        }        
    }
}
