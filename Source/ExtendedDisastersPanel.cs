﻿using UnityEngine;
using ColossalFramework;
using ColossalFramework.UI;

namespace EnhancedDisastersMod
{
    public class ExtendedDisastersPanel : UIPanel
    {
        private UILabel[] labels;
        private UIProgressBar[] progressBars_probability;
        private UIProgressBar[] progressBars_maxIntensity;
        private string labelFormat = "{0} ({1:0.00}/{2})";
        public int Counter = 0;

        public override void Awake()
        {
            base.Awake();

            //this.backgroundSprite = "GenericPanel";
            //this.color = new Color32(255, 0, 0, 100);
            this.backgroundSprite = "MenuPanel";
            this.canFocus = true;
            //this.isInteractive = true;

            height = 200;
            width = 400;

            isVisible = false;
        }

        public override void Start()
        {
            base.Start();

            UILabel lTitle = this.AddUIComponent<UILabel>();
            lTitle.position = new Vector3(10, -15);
            lTitle.text = "Disasters info";

            int y = -50;
            int h = -20;

            addAxisTitle(200, y, "Probability");
            addAxisTitle(300, y, "Max intensity");
            y -= 15;

            addAxisLabel(200, y, "0.1");
            addAxisLabel(240, y, "1");
            addAxisLabel(275, y, "10");
            addAxisLabel(300, y, "1");
            addAxisLabel(375, y, "10");
            y -= 15;

            int disasterCount = Singleton<EnhancedDisastersManager>.instance.container.AllDisasters.Count;
            labels = new UILabel[disasterCount];
            progressBars_probability = new UIProgressBar[disasterCount];
            progressBars_maxIntensity = new UIProgressBar[disasterCount];

            EnhancedDisastersManager edm = Singleton<EnhancedDisastersManager>.instance;
            for (int i = 0; i < disasterCount; i++)
            {
                EnhancedDisaster d = edm.container.AllDisasters[i];
                labels[i] = addLabel(10, y);
                labels[i].text = string.Format(labelFormat, d.GetName(), 0, 0);
                progressBars_probability[i] = addProgressBar(200, y);
                progressBars_maxIntensity[i] = addProgressBar(300, y);
                y += h;
            }

            UIButton bigRedBtn = this.AddUIComponent<UIButton>();
            bigRedBtn.name = "bigRedBtn";
            bigRedBtn.position = new Vector3(10, height - 30);
            bigRedBtn.size = new Vector2(width - 20, 25);
            bigRedBtn.textColor = Color.white;
            bigRedBtn.normalBgSprite = "ButtonMenu";
            bigRedBtn.hoveredBgSprite = "ButtonMenuHovered";
            bigRedBtn.text = "Stop all disasters";
            bigRedBtn.eventClick += BigRedBtn_eventClick;

            UIButton btn = this.AddUIComponent<UIButton>();
            btn.position = new Vector3(370, -5);
            btn.size = new Vector2(30, 30);
            btn.normalFgSprite = "buttonclose";
            btn.eventClick += Btn_eventClick;
        }

        private void BigRedBtn_eventClick(UIComponent component, UIMouseEventParameter eventParam)
        {
            DisasterManager dm = Singleton<DisasterManager>.instance;
            for (ushort i = 0; i < dm.m_disasterCount; i++)
            {
                dm.ReleaseDisaster(i);
            }
        }

        private void Btn_eventClick(UIComponent component, UIMouseEventParameter eventParam)
        {
            this.Hide();
        }

        private UILabel addLabel(int x, int y)
        {
            UILabel l = this.AddUIComponent<UILabel>();
            l.position = new Vector3(x, y);
            l.textScale = 0.8f;

            return l;
        }

        private void addAxisLabel(int x, int y, string text)
        {
            //switch (labelTextAlignment)
            //{
            //    case UIHorizontalAlignment.Center:
            //        x -= 15;
            //        break;
            //    case UIHorizontalAlignment.Right:
            //        x -= 30;
            //        break;
            //}

            UILabel l = this.AddUIComponent<UILabel>();
            l.position = new Vector3(x, y);
            l.textScale = 0.7f;
            l.text = text;
        }

        private void addAxisTitle(int x, int y, string text)
        {
            UILabel l = this.AddUIComponent<UILabel>();
            l.position = new Vector3(x, y);
            l.textScale = 0.7f;
            l.text = text;
        }

        private UIProgressBar addProgressBar(int x, int y)
        {
            UIProgressBar b = this.AddUIComponent<UIProgressBar>();
            b.backgroundSprite = "LevelBarBackground";
            b.progressSprite = "LevelBarForeground";
            b.progressColor = Color.red;
            b.position = new Vector3(x, y);
            b.width = 90;
            b.value = 0.5f;

            return b;
        }

        private float getProgressValueLog(float value)
        {
            if (value <= 0.1) return 0;
            if (value >= 10) return 1;
            return (1f + Mathf.Log10(value)) / 2f;
        }

        public override void Update()
        {
            base.Update();

            if (!isVisible) return;

            if (--Counter > 0) return;
            Counter = 200;

            EnhancedDisastersManager edm = Singleton<EnhancedDisastersManager>.instance;
            int disasterCount = edm.container.AllDisasters.Count;

            for (int i = 0; i < disasterCount; i++)
            {
                EnhancedDisaster d = edm.container.AllDisasters[i];
                float p = d.GetCurrentOccurrencePerYear();
                byte maxIntensity = d.GetMaximumIntensity();
                if (d.Enabled)
                {
                    labels[i].text = string.Format(labelFormat, d.GetName(), p, maxIntensity);

                    progressBars_probability[i].value = getProgressValueLog(p);
                    setProgressBarColor(progressBars_probability[i]);

                    progressBars_maxIntensity[i].value = maxIntensity * 0.01f;
                    setProgressBarColor(progressBars_maxIntensity[i]);
                }
                else
                {
                    labels[i].text = "Disabled";

                    progressBars_probability[i].value = 0;
                    progressBars_probability[i].progressColor = Color.black;

                    progressBars_maxIntensity[i].value = 0;
                    progressBars_maxIntensity[i].progressColor = Color.black;
                }
            }
        }

        private void setProgressBarColor(UIProgressBar progressBar)
        {
            float value = progressBar.value;
            progressBar.progressColor = new Color(2.0f * value, 2.0f * (1 - value), 0);
        }
    }
}
