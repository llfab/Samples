// ===========================================================================\r
//                          B I T S   O F   N A T U R E\r
// ===========================================================================\r
//  This document contains proprietary information. It is the exclusive\r
//  confidential property of Stryker Corporation and its affiliates.\r
// \r
//  Copyright (c) 2024 Stryker\r
// ===========================================================================

using AvaloniaIosApp.Models;
using AvaloniaIosApp.ViewModels;

namespace AvaloniaIosApp
{
    public  class AvaloniaIosAppSystem
    {
        public static AvaloniaIosAppSystem Instance { get; private set; }

        private readonly MainModel _mainModel;
        public MainModel MainModel => _mainModel;
        
        private readonly MainViewModel _mainViewModel;
        public MainViewModel MainViewModel => _mainViewModel;
        
        public AvaloniaIosAppSystem()
        {
            _trace = Trace.ForType<AvaloniaIosAppSystem>();
            Instance = this;
            _mainModel = new MainModel();
            _mainViewModel = new MainViewModel();
        }
    }
}
