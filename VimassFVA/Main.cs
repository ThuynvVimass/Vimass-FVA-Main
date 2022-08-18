using Luxand;
using Newtonsoft.Json;
using System;
using System.Drawing;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Diagnostics;

namespace VimassFVA
{
    public partial class Main : Form
    {
        static string path = "D:\\Vimass\\dataFAV.txt";
        static string pathSoVi = "D:\\Vimass\\dataViFAV.txt";

        bool needClose = false;
        String cameraName;

        FSDK.TFacePosition facePosition_Global;
        bool isLiveness = false;
        byte[] template_Global;
        int image_Global;
        static byte[] template_FromFile;
        static String soVi_FromFile;
        /*static String soVi;*/
        static bool daDocThongTinTuFile = false;

        int soLanXacThuc;
        static String KEYMD5 = "jdsf8weur93r93jr30r0wffk2";

        public Main()
        {
            InitializeComponent();
        }

        private void Main_Load(object sender, EventArgs e)
        {
            string soVi = LoadDataFromFile();
            Debug.WriteLine("so vi da luu: " + soVi);
            if (!soVi.Equals(""))
            {
                new Authen(soVi).Show();
                this.WindowState = FormWindowState.Minimized;
                this.ShowInTaskbar = false;
            }
            else
            {
                new LoginForm().Show();
                this.WindowState = FormWindowState.Minimized;
                this.ShowInTaskbar = false;
            }
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
            System.Diagnostics.Debugger.Launch();
            int VideoFormat = 0; // Chọn một định dạng video
            
        }

        private String LoadDataFromFile()
        {
            String soVi = "";
            try
            {
                if (File.Exists(Ulti.path) && File.Exists(Ulti.pathSoVi))
                { 
                    soVi_FromFile = docThongTin(pathSoVi);
                    Debug.WriteLine("soVi_FromFile : " + soVi_FromFile);
                    string temp = docThongTin(path, soVi_FromFile);
                    template_FromFile = Encoding.UTF8.GetBytes(temp);

                    if (template_FromFile == null || template_FromFile == new byte[FSDK.TemplateSize])
                    {                       
                        daDocThongTinTuFile = false;                                        
                    }
                    else
                    {                        
                        daDocThongTinTuFile = true;                       
                        soVi = soVi_FromFile;
                        Ulti.soVi = soVi;
                        Ulti.templateVi = template_FromFile;
                    }
                }



                
                /*if (soVi_FromFile.CompareTo(soVi) == 0) 
                {
                    if (template_FromFile == null || template_FromFile == new byte[FSDK.TemplateSize])
                    {
                        *//*btn_dangKy.Enabled = true;
                        btn_xacThuc.Enabled = false;*//*
                        daDocThongTinTuFile = false;
                        *//*lb_thongBao.Text = "\"Số ví không có dữ liệu khuôn mặt\nVui lòng đăng ký thông tin\"";
                        lb_thongBao.ForeColor = System.Drawing.Color.Red;                        *//*
                    }
                    else
                    {
                       *//* btn_dangKy.Enabled = false;
                        btn_xacThuc.Enabled = true;*//*
                        daDocThongTinTuFile = true;
                       *//* lb_thongBao.Text = "\"Đã có dữ liệu khuôn mặt\nVui lòng xác thực\"";
                        lb_thongBao.ForeColor = System.Drawing.SystemColors.ActiveCaptionText;*//*
                        soVi = soVi_FromFile;
                    }
                } 
                else
                {
                   *//* btn_dangKy.Enabled = true;
                    btn_xacThuc.Enabled = false;
                    lb_thongBao.Text = "\"Số ví không có dữ liệu khuôn mặt\nVui lòng đăng ký thông tin\"";
                    lb_thongBao.ForeColor = System.Drawing.Color.Red;              *//*     
                }*/
                
            }
            catch (System.IO.DirectoryNotFoundException e)
            {
                string directoryPath = "D:\\Vimass";
                DirectoryInfo directory = new DirectoryInfo(directoryPath);
                directory.Create();
                directory.Attributes = FileAttributes.Hidden;
             /*   btn_dangKy.Enabled = true;
                btn_xacThuc.Enabled = false;*/
                daDocThongTinTuFile = false;
             /*   lb_thongBao.Text = "\"Số ví không có dữ liệu khuôn mặt\nVui lòng đăng ký thông tin\"";
                lb_thongBao.ForeColor = System.Drawing.Color.Red;                               */
            }
            catch (Exception e)
            {
             /*   btn_dangKy.Enabled = true;
                btn_xacThuc.Enabled = false;*/
                daDocThongTinTuFile = false;
             /*   lb_thongBao.Text = "\"Số ví không có dữ liệu khuôn mặt\nVui lòng đăng ký thông tin\"";
                lb_thongBao.ForeColor = System.Drawing.Color.Red;                               */
            }
            return soVi;
        }

        private void btn_khoiDongCamera_Click(object sender, EventArgs e)
        {
           /* soVi = txt_soVi.Text;*/
            LoadDataFromFile();
            khoiDongCamera();
        }

        private void khoiDongCamera()
        {
            
            /*this.btn_khoiDongCamera.Enabled = false;*/
            int cameraHandle = 0;

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
                Debug.WriteLine("OK");
                Int32 imageHandle = 0;
                if (FSDK.FSDKE_OK != FSDKCam.GrabFrame(cameraHandle, ref imageHandle)) // grab the current frame from the camera
                {
                    Application.DoEvents();
                    continue;
                }
                FSDK.CImage image_Global = new FSDK.CImage(imageHandle);

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
                /*pictureBox1.Image = frameImage;*/
                GC.Collect(); // collect the garbage after the deletion
            }

            FSDK.FreeTracker(tracker);
            FSDKCam.CloseVideoCamera(cameraHandle);
            FSDKCam.FinalizeCapturing();
        }

        

        private void btn_dangKiKhuonMat_Click(object sender, EventArgs e)
        {
            dangKy();
        }

        private void dangKy()
        {
            int res2 = FSDK.DetectFace(image_Global, ref facePosition_Global);
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
                    FSDK.GetFaceTemplate(image_Global, out template_Global);
                    try
                    {
                        luuThongTin(path, template_Global, pathSoVi, Ulti.soVi);
                        template_FromFile = template_Global;
                        daDocThongTinTuFile = true;
                       /* lb_thongBao.Text = "\"Đăng kí khuôn mặt thành công" +
                              "\r\nCho số ví: " + soVi + "\"";
                        lb_thongBao.ForeColor = System.Drawing.SystemColors.ActiveCaptionText;*/
                        MessageBox.Show("Đã lưu thông tin templace", "Thông tin", MessageBoxButtons.OK, MessageBoxIcon.Information);
                       /* btn_xacThuc.Enabled = true;
                        btn_dangKy.Enabled = false;*/
                    }
                    catch (Exception)
                    {
                       /* lb_thongBao.Text = "\"Đăng kí khuôn mặt không thành công thành công" +
                              "\r\nLỗi khi lưu thông tin\"";
                        lb_thongBao.ForeColor = System.Drawing.Color.Red;*/

                    }
                }
            }
        }

        private void btn_xacThuc_Click(object sender, EventArgs e)
        {
            xacThuc();
        }
        private void xacThuc()
        {

            /*lb_thongBao.Text = "\"\"";*/
            if (soLanXacThuc < 3)
            {
                int res2 = FSDK.DetectFace(image_Global, ref facePosition_Global);
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
                        FSDK.GetFaceTemplate(image_Global, out template_Global);
                        if (daDocThongTinTuFile)
                        {
                            float matchingThreshold = 0;
                            float similarity = 0;
                            FSDK.GetMatchingThresholdAtFAR((float)0.02, ref matchingThreshold);
                            FSDK.MatchFaces(ref template_Global, ref template_FromFile, ref similarity);
                            if (similarity > matchingThreshold)
                            {
                               /* lb_thongBao.Text = "\"Xác thực khuôn mặt thành công!\"";
                                lb_thongBao.ForeColor = System.Drawing.SystemColors.ActiveCaptionText;*/
                                soLanXacThuc = 0;
                                // Lưu dữ liệu khuôn mặt mới vào file
                                luuThongTin(path, template_Global, pathSoVi, Ulti.soVi);
                                guiThongTinLenServer(Ulti.soVi, 1);
                            }
                            else
                            {
                                soLanXacThuc++;
                               /* lb_thongBao.Text = "\"Xác thực khuôn mặt thất bại, lần " + soLanXacThuc +"\"";
                                lb_thongBao.ForeColor = System.Drawing.Color.Red;*/
                            }
                        }
                    }
                } 
            }
            else
            {
               /* lb_thongBao.Text = "\"Bạn đã xác thực quá 3 lần, Vui lòng thử lại sau\"";
                lb_thongBao.ForeColor = System.Drawing.Color.Red;*/
                guiThongTinLenServer(Ulti.soVi, -1);
            }
        }

        private void guiThongTinLenServer(string soVi, int identity)
        {
            IdentityFVA identityFVA = new IdentityFVA();
            identityFVA.identity = identity;
            identityFVA.soVi = soVi;
            identityFVA.checksum = MaHoaDuLieu.getMD5(soVi + identity + KEYMD5);
            var json = JsonConvert.SerializeObject(identityFVA);
            String urlService = "http://localhost:8082/vimass-tmdt/services/identity-fva";
            String result = Service.SendWebrequest_POST_Method(json,urlService);
        }

        private void luuThongTin(string path, byte[] template_Global, string pathSoVi, string soVi)
        {
            string soVi_MaHoa = MaHoaDuLieu.maHoa(soVi);
            File.WriteAllText(pathSoVi, soVi_MaHoa);

            string temp = Encoding.ASCII.GetString(template_Global);
            string template_Global_MaHoa = MaHoaDuLieu.maHoa(temp, soVi);
            File.WriteAllText(path, template_Global_MaHoa);
        }

        private String docThongTin(string path)
        {
            string temp = File.ReadAllText(path);
            return MaHoaDuLieu.giaiMa(temp);
        }
        private String docThongTin(string path, string key)
        {
            string temp = File.ReadAllText(path);
            return MaHoaDuLieu.giaiMa(temp, key);
        }

        private void Main_FormClosing(object sender, FormClosingEventArgs e)
        {
            needClose = true;
        }

        private void Main_VisibleChanged(object sender, EventArgs e)
        {
            
        }

        private void Main_Shown(object sender, EventArgs e)
        {
            /*base.OnShown(e);
            string soVi = LoadDataFromFile();
            if (!soVi.Equals(""))
            {
                new Authen(soVi).Show();
                this.WindowState = FormWindowState.Minimized;
                this.ShowInTaskbar = false;
            }
            else
            {
                new LoginForm().Show();
                this.WindowState = FormWindowState.Minimized;
                this.ShowInTaskbar = false;
            }*/
        }
    }
}
