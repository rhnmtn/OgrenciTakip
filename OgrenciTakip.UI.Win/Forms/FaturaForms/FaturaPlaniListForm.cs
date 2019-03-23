﻿
using OgrenciTakip.BLL.General;
using OgrenciTakip.Common.Enums;
using OgrenciTakip.UI.Win.Forms.BaseForms;
using OgrenciTakip.UI.Win.GeneralForms;
using OgrenciTakip.UI.Win.Functions;
using OgrenciTakip.Model.DTO;
using OgrenciTakip.Common.Message;
using OgrenciTakip.UI.Win.Show;
using DevExpress.XtraBars;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Linq;

namespace OgrenciTakip.UI.Win.Forms.FaturaForms
{
    public partial class FaturaPlaniListForm : BaseListForm
    {
        public FaturaPlaniListForm()
        {
            InitializeComponent();
            bll = new TahakkukBll();
            HideItems = new BarItem[] { btnYeni, barInsert, barInsertAciklama, barDelete, barDeleteAciklama, btnAktifPasifKartlar };
            ShowItems = new BarItem[] { btnTahakkukYap };
            btnSil.Caption = "Fatura Planı İptal Et";
            btnTahakkukYap.Caption = "Toplu Fatura Planı";
            btnYazdir.CreateDropDownMenu(new BarItem[] { btnTabloYazdir });

        }

        protected override void DegiskenleriDoldur()
        {
            Tablo = tablo;
            kartTuru = KartTuru.Fatura;
            navigator = longNavigator.Navigator;
        }

        protected override void Listele()
        {
            Tablo.GridControl.DataSource = ((TahakkukBll)bll).FaturaTahakkukList(x => x.SubeId == AnaForm.SubeId && x.DonemId == AnaForm.DonemId);
        }

        protected override void ShowEditForm(long id)
        {
            var entity = tablo.GetRow<FaturaL>();
            if (entity == null) return;

            if (entity.HizmetNetTutar == 0)
            {
                Messages.HataMesaji("Öğrencinin Net Ücreti Sıfır ( 0 ) Olduğu İçin Fatura Planı Oluşturamazsınız."); return;
            }

            var result = ShowEditForms<FaturaPlaniEditForm>.ShowDialogEditForms(KartTuru.Fatura, id, null);
            ShowEditFormDefault(result);
        }

        protected override void TahakkukYap()
        {
            var source = new List<FaturaL>();

            for (int i = 0; i < tablo.DataRowCount; i++)
                source.Add(tablo.GetRow<FaturaL>(i));

            if (source.Count == 0) return;

            if (ShowEditForms<TopluFaturaPlaniEditForm>.ShowDialogEditForms(KartTuru.Fatura, source))
                Listele();
        }

        protected override void EntityDelete()
        {
            if (Messages.HayirSeciliEvetHayir("Seçilen Öğrencilere Ait Hareket Görmeyen Tüm Fatura Planları İptal Edilecektir. Onaylıyor Musunuz", "İptal Onay") != DialogResult.Yes) return;

            var source = new List<FaturaL>();

            for (int i = 0; i < tablo.DataRowCount; i++)
                source.Add(tablo.GetRow<FaturaL>(i));
            if (source.Count == 0) return;

            using (var bll = new FaturaBll())
            {
                var position = 0.0;
                progressBarControl.Visible = true;
                progressBarControl.Left = (ClientSize.Width - progressBarControl.Width) / 2;
                progressBarControl.Top = (ClientSize.Height - progressBarControl.Height) / 2;

                source.ForEach(x =>
                {
                    var yuzde = 100.0 / source.Count;
                    position += yuzde;
                    var planSource = bll.List(y => y.TahakkukId == x.Id).Where(y => ((FaturaPlaniL)y).TahakkukTarih == null).ToList();
                    bll.Delete(planSource);

                    progressBarControl.Position = (int)position;
                    progressBarControl.Update();
                });
            }

            progressBarControl.Visible = false;
            Messages.BilgiMesaji("Seçilen Öğrencilere Ait Fatura Planları Başarılı Bir Şekilde İptal Edilmiştir.");
            Listele();
        }

    }
}