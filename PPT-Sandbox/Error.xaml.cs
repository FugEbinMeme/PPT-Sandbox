﻿using System.Globalization;

namespace Sandbox {
    public partial class Error {
        public Error(string exceptionText) {
            InitializeComponent();

            switch (CultureInfo.CurrentCulture.TwoLetterISOLanguageName) {
                case "ko":
                    General.Text = "PPT-Sandbox에 에러가 발생했습니다. 아래의 정보를 개발자에게 신고해주세요.";
                    break;
                    
                case "ja":
                    General.Text = "エラーが発生しました。以下の情報を開発者に報告してください。";
                    break;
                    
                default:
                    General.Text = "PPT-Sandbox has encountered an error. Please report the information below to the developers.";
                    break;
            }

            Exception.Text = $"Version: {App.VersionString}\n\n{exceptionText}";
        }
    }
}
