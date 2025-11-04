
using System;
using System.Collections.Generic;
using System.Linq;
using Foundation;
using AppKit;

using ObjCRuntime;


//using System;
//using System.Drawing;
//using MonoTouch.Foundation;
//using MonoTouch.UIKit;
using WBid.WBidiPad.Core;

//using MonoTouch.CoreGraphics;
using WBid.WBidiPad.PortableLibrary.Utility;
using WBid.WBidiPad.iOS;
using WBid.WBidiPad.Model;

//using System.Collections.Generic;
using WBid.WBidiPad.PortableLibrary.BusinessLogic;

//using System.Linq;
using WBid.WBidiPad.iOS.Utility;
using System.Collections.ObjectModel;
using System.IO;

//using MonoTouch.EventKit;
//using System.Collections.ObjectModel;
using WBid.WBidiPad.PortableLibrary;
using VacationCorrection;
using WBid.WBidiPad.SharedLibrary.Utility;
using System.IO.Compression;
using System.Net;
using WBid.WBidiPad.Core.Enum;
using WBid.WBidiPad.Model.State.Weights;
using WBid.WBidiPad.SharedLibrary.Parser;
using System.Globalization;
using System.Runtime.Serialization.Json;
using System.Text;
using CoreGraphics;
using Newtonsoft.Json.Linq;
using WBid.WBidMac.Mac.WindowControllers;
using WBid.WBidMac.Mac.WindowControllers.SynchView;
using WBid.WBidMac.Mac.WindowControllers.SynchSelectionView;
using System.Threading.Tasks;
using WBid.WBidiPad.SharedLibrary.SWA;
using WBid.WBidMac.Mac.WindowControllers.VacationDifference;
using WBid.WBidMac.Mac.WindowControllers.CommuteDifference;
using ADT.Common.Models;
using ADT.Engine.Mapper;
using System.Text.Json;

namespace WBid.WBidMac.Mac
{

    


    public partial class MainWindowController : AppKit.NSWindowController
    {

        readonly NSString themeKeyString = new NSString("AppleInterfaceThemeChangedNotification");
        readonly NSString dark = new NSString("Dark");
        readonly Selector modeSelector = new Selector("themeChanged:");
        SummaryViewController summaryVC;
        BidLineViewController bidlineVC;
        ModernViewController modernVC;
        CalendarWindowController calendarWC;
        WBidState wBIdStateContent;
        public CSWWindowController cswWC;
        public SecretDataDownloadController secretDataView;
        BAWindowController baWC;
        // synch view 
        public SynchViewWindowController synchView;

        NSPanel overlayPanel;
        OverlayViewController overlay;
        NSObject notif;
        NSObject notifClos;
        Dictionary<string, Trip> trips = null;
        Dictionary<string, Line> lines = null;
        NSObject confNotif;
        NSObject reloadNotification;
        NSObject modeChangeNoti;
        bool FirstTime;
        bool SynchBtn;
        int isNeedToClose;
        bool isNeedtoCreateMILFile = false;
        public System.Timers.Timer timer;
        QuickSetWindowController qsWC;
        PairingWindowController pairing ;
        int DataSynchSelecedValue = 0; //1 for state, 2 for quickset and 3 for both
        bool IsStateFromServer = false;
        bool IsQSFromServer = false;
        bool IskeepLocalQS = false;
        bool IskeepLocalState = false;
        // notification for synch added by Francis 22 jul 2020

        NSObject synchNotif;

        #region Constructors

        // Called when created from unmanaged code
        public MainWindowController (NativeHandle handle) : base (handle)
        {
            Initialize ();
        }
        
        // Called when created directly from a XIB file
        [Export ("initWithCoder:")]
        public MainWindowController (NSCoder coder) : base (coder)
        {
            Initialize ();
        }

        // Call to load from the XIB/NIB file
        public MainWindowController() : base("MainWindow")
        {
            Initialize();

        }

        // Shared initialization code
        void Initialize ()
        {

        }

     

        

        #endregion

        //strongly typed window accessor
        public new MainWindow Window {
            get {
                return (MainWindow)base.Window;
            }
        }

        static NSButton closeButton;

      
        [Export("themeChanged:")]
        public void ThemeChanged(NSObject change)
        {
            var interfaceStyle = NSUserDefaults.StandardUserDefaults.StringForKey("AppleInterfaceStyle");
            if (interfaceStyle == "Dark")
            {
                btnHome.Image = NSImage.ImageNamed("homeMacDark.png");
                btnSave.Image = NSImage.ImageNamed("saveMacDark.png");
                btnSynch.Image = NSImage.ImageNamed("synchIconDark.png");
            }
            else
            {
                btnHome.Image = NSImage.ImageNamed("homeMac.png");
                btnSave.Image = NSImage.ImageNamed("saveMac.png");
                btnSynch.Image = NSImage.ImageNamed("synchIcon3.png");
            }
        }

        // Added by Gregory on 21/12/2020 custom method for changing the oush button title color.
        void TextColor(NSButton button, NSColor color) {
            var coloredTitle = new NSMutableAttributedString(button.Title);
            var titleRange = new NSRange(0, coloredTitle.Length);
            coloredTitle.AddAttribute(NSStringAttributeKey.ForegroundColor, color, titleRange);
            button.AttributedTitle = coloredTitle;
            
        }


        public override void AwakeFromNib ()
        {
            CommonClass.AppDelegate.HandleViewCap();
            base.AwakeFromNib();
            //string submitResult = "21221\n1\n2\n3\n4\n5\n6\n7\n8\n9\n10\n11\n12\n13\n14\n15\n16\n17\n18\n19\n20\n21\n22\n23\n24\n25\n26\n27\n28\n29\n30\n31\n32\n33\n34\n35\n36\n37\n38\n39\n40\n41\n42\n43\n44\n45\n46\n47\n48\n49\n50\n51\n52\n53\n54\n55\n56\n57\n58\n59\n60\n61\n62\n63\n64\n65\n66\n67\n68\n69\n70\n71\n72\n73\n74\n75\n76\n77\n78\n79\n80\n81\n82\n83\n84\n85\n86\n87\n88\n89\n90\n91\n92\n93\n94\n95\n96\n97\n98\n99\n100\n101\n102\n103\n104\n105\n106\n107\n108\n109\n110\n111\n112\n113\n114\n115\n116\n117\n118\n119\n120\n121\n122\n123\n124\n125\n126\n127\n128\n129\n130\n131\n132\n133\n134\n135\n136\n137\n138\n139\n140\n141\n142\n143\n144\n145\n146\n147\n148\n149\n150\n151\n152\n153\n154\n155\n156\n157\n158\n159\n160\n161\n162\n163\n164\n165\n166\n167\n168\n169\n170\n171\n172\n173\n174\n175\n176\n177\n178\n179\n180\n181\n182\n183\n184\n185\n186\n187\n188\n189\n190\n191\n192\n193\n194\n195\n196\n197\n198\n199\n200\n201\n202\n203\n204\n205\n206\n207\n208\n209\n210\n211\n212\n213\n214\n215\n216\n217\n218\n219\n220\n221\n222\n223\n224\n225\n226\n227\n228\n229\n230\n231\n232\n233\n234\n235\n236\n237\n238\n239\n240\n241\n242\n243\n244\n245\n246\n247\n248\n249\n250\n251\n252\n253\n254\n255\n256\n257\n258\n259\n260\n261\n262\n263\n264\n265\n266\n267\n268\n269\n270\n271\n272\n273\n274\n275\n276\n277\n278\n279\n280\n281\n282\n283\n284\n285\n286\n287\n288\n289\n290\n291\n292\n293\n294\n295\n296\n297\n298\n299\n300\n301\n302\n303\n304\n305\n306\n307\n308\n309\n310\n311\n312\n313\n314\n315\n316\n317\n318\n319\n320\n11\n12\n13\n14\n15\n16\n17\n18\n19\n20\n21\n22\n23\n24\n25\n26\n27\n28\n29\n30\n31\n32\n33\n34\n35\n36\n37\n38\n39\n40\n41\n42\n43\n44\n45\n46\n47\n48\n49\n50\n51\n52\n53\n54\n55\n56\n57\n58\n59\n60\n61\n62\n63\n64\n65\n66\n67\n68\n69\n470\n471\n314\n315\n316\n317\n318\n319\n320\n11\n12\n13\n14\n15\n16\n17\n18\n19\n20\n21\n22\n23\n24\n25\n26\n27\n28\n29\n30\n31\n32\n33\n34\n35\n36\n37\n38\n39\n40\n41\n42\n43\n44\n45\n46\n47\n48\n49\n50\n51\n52\n53\n54\n55\n56\n57\n58\n59\n60\n61\n62\n63\n64\n65\n66\n67\n68\n69\n570\n1\n2\n3\n4\n5\n6\n7\n8\n9\n10\n11\n12\n13\n14\n15\n16\n17\n18\n19\n20\n21\n22\n23\n24\n25\n26\n27\n28\n29\n30\n31\n32\n33\n34\n35\n36\n37\n38\n39\n40\n41\n42\n43\n44\n45\n46\n47\n48\n49\n50\n51\n52\n53\n54\n55\n56\n57\n58\n59\n60\n61\n62\n63\n64\n65\n66\n67\n68\n69\n70\n71\n72\n73\n74\n75\n76\n77\n78\n79\n80\n81\n82\n83\n84\n85\n86\n87\n88\n89\n90\n91\n92\n93\n94\n95\n96\n97\n98\n99\n100\n101\n102\n103\n104\n105\n106\n107\n108\n109\n110\n111\n112\n113\n114\n115\n116\n117\n118\n119\n120\n121\n122\n123\n124\n125\n126\n127\n128\n129\n130\n131\n132\n133\n134\n135\n136\n137\n138\n139\n140\n141\n142\n143\n144\n145\n146\n147\n148\n149\n150\n151\n152\n153\n154\n155\n156\n157\n158\n159\n160\n161\n162\n163\n164\n165\n166\n167\n168\n169\n170\n171\n172\n173\n174\n175\n176\n177\n178\n179\n180\n181\n182\n183\n184\n185\n186\n187\n188\n189\n190\n191\n192\n193\n194\n195\n196\n197\n198\n199\n200\n201\n202\n203\n204\n205\n206\n207\n208\n209\n210\n211\n212\n213\n214\n215\n216\n217\n218\n219\n220\n221\n222\n223\n224\n225\n226\n227\n228\n229\n230\n231\n232\n233\n234\n235\n236\n237\n238\n239\n240\n241\n242\n243\n244\n245\n246\n247\n248\n249\n250\n251\n252\n253\n254\n255\n256\n257\n258\n259\n260\n261\n262\n263\n264\n265\n266\n267\n268\n269\n270\n271\n272\n273\n274\n275\n276\n277\n278\n279\n280\n281\n282\n283\n284\n285\n286\n287\n288\n289\n290\n291\n292\n293\n294\n295\n296\n297\n298\n299\n300\n301\n302\n303\n304\n305\n306\n307\n308\n309\n310\n311\n312\n313\n314\n315\n316\n317\n318\n319\n320\n11\n12\n13\n14\n15\n16\n17\n18\n19\n20\n21\n22\n23\n24\n25\n26\n27\n28\n29\n30\n31\n32\n33\n34\n35\n36\n37\n38\n39\n40\n41\n42\n43\n44\n45\n46\n47\n48\n49\n50\n51\n52\n53\n54\n55\n56\n57\n58\n59\n60\n61\n62\n63\n64\n65\n66\n67\n68\n69\n470\n471\n314\n315\n316\n317\n318\n319\n320\n11\n12\n13\n14\n15\n16\n17\n18\n19\n20\n21\n22\n23\n24\n25\n26\n27\n28\n29\n30\n31\n32\n33\n34\n35\n36\n37\n38\n39\n40\n41\n42\n43\n44\n45\n46\n47\n48\n49\n50\n51\n52\n53\n54\n55\n56\n57\n58\n59\n60\n61\n62\n63\n64\n65\n66\n67\n68\n69\n570\n*E\n*E\n SUBMITTED BY: [e21221]     23748    06/06/22 07:57:40\r\n";
            //string submitResult = "21221\n79\n80\n81\n82\n83\n84\n85\n86\n87\n88\n89\n90\n91\n92\n93\n94\n95\n96\n97\n98\n99\n100\n101\n102\n103\n104\n105\n106\n107\n108\n109\n110\n111\n112\n113\n114\n115\n116\n117\n118\n119\n120\n121\n122\n123\n124\n125\n126\n127\n128\n129\n130\n131\n132\n133\n134\n135\n136\n137\n138\n139\n140\n141\n142\n143\n144\n145\n146\n147\n148\n149\n150\n151\n152\n153\n154\n155\n156\n157\n158\n159\n160\n161\n162\n163\n164\n165\n166\n167\n168\n169\n170\n171\n172\n173\n174\n175\n176\n177\n178\n179\n180\n181\n182\n183\n184\n185\n186\n187\n188\n189\n190\n191\n192\n193\n194\n195\n196\n197\n198\n199\n200\n201\n202\n203\n204\n205\n206\n207\n208\n209\n210\n211\n212\n213\n214\n215\n216\n217\n218\n219\n220\n221\n222\n223\n224\n225\n226\n227\n228\n229\n230\n231\n232\n233\n234\n235\n236\n237\n238\n239\n240\n241\n242\n243\n244\n245\n246\n247\n248\n249\n250\n251\n252\n253\n254\n255\n256\n257\n258\n259\n260\n261\n262\n263\n264\n265\n266\n267\n268\n269\n270\n271\n272\n273\n274\n275\n276\n277\n278\n279\n280\n281\n282\n283\n284\n285\n286\n287\n288\n289\n290\n291\n292\n293\n294\n295\n296\n297\n298\n299\n300\n301\n302\n303\n304\n305\n306\n307\n308\n309\n310\n311\n312\n313\n314\n315\n316\n317\n318\n319\n320\n321\n322\n323\n324\n325\n326\n327\n328\n329\n330\n331\n332\n333\n334\n335\n336\n337\n338\n339\n340\n341\n342\n343\n344\n345\n346\n347\n348\n349\n350\n351\n352\n353\n354\n355\n356\n357\n358\n359\n360\n361\n362\n363\n364\n365\n366\n367\n368\n369\n370\n371\n372\n373\n374\n375\n376\n377\n378\n379\n380\n381\n382\n383\n384\n385\n386\n387\n388\n389\n390\n391\n392\n393\n394\n395\n396\n397\n398\n399\n400\n401\n402\n403\n404\n405\n406\n407\n408\n409\n410\n411\n412\n413\n414\n415\n416\n417\n418\n419\n420\n421\n422\n423\n424\n425\n426\n427\n428\n429\n430\n431\n432\n433\n434\n435\n436\n437\n438\n439\n440\n441\n442\n443\n444\n445\n446\n447\n448\n449\n450\n451\n452\n453\n454\n455\n456\n457\n458\n459\n460\n461\n462\n463\n464\n465\n466\n467\n468\n469\n470\n471\n472\n473\n474\n475\n476\n477\n478\n479\n480\n481\n482\n483\n484\n485\n486\n487\n488\n489\n490\n491\n492\n493\n494\n495\n496\n497\n498\n499\n500\n501\n502\n503\n504\n505\n506\n507\n508\n509\n510\n511\n512\n513\n514\n515\n516\n517\n518\n519\n520\n521\n522\n523\n524\n525\n526\n527\n528\n529\n530\n531\n532\n533\n534\n535\n536\n537\n538\n539\n540\n541\n542\n543\n544\n545\n546\n547\n548\n549\n550\n551\n552\n553\n554\n555\n556\n557\n558\n559\n560\n561\n562\n563\n564\n565\n566\n567\n568\n569\n570\n571\n572\n573\n574\n575\n576\n577\n578\n579\n580\n581\n582\n583\n584\n585\n586\n587\n588\n589\n590\n591\n592\n593\n594\n595\n596\n597\n598\n599\n600\n601\n602\n603\n604\n605\n606\n607\n608\n609\n610\n611\n612\n613\n614\n615\n616\n617\n618\n619\n620\n621\n622\n623\n624\n625\n626\n627\n628\n629\n630\n631\n632\n633\n634\n635\n636\n637\n638\n639\n640\n641\n642\n643\n644\n645\n646\n647\n648\n649\n650\n651\n652\n653\n654\n655\n656\n657\n658\n659\n660\n661\n662\n663\n664\n665\n666\n667\n668\n669\n670\n671\n672\n673\n674\n675\n676\n677\n678\n679\n680\n681\n682\n683\n684\n685\n686\n687\n688\n689\n690\n691\n692\n693\n694\n695\n696\n697\n698\n699\n700\n701\n702\n703\n704\n705\n706\n707\n708\n709\n710\n711\n712\n713\n714\n715\n716\n717\n718\n719\n720\n721\n722\n723\n724\n725\n726\n727\n728\n729\n730\n731\n732\n733\n734\n735\n736\n737\n738\n739\n740\n741\n742\n743\n744\n745\n746\n747\n748\n749\n750\n751\n752\n753\n754\n755\n756\n757\n758\n759\n760\n761\n762\n763\n764\n765\n766\n767\n768\n769\n770\n771\n772\n773\n774\n775\n776\n777\n778\n779\n780\n781\n782\n783\n784\n785\n786\n787\n788\n789\n790\n791\n792\n793\n794\n795\n796\n797\n798\n799\n800\n801\n802\n803\n804\n805\n806\n807\n808\n809\n810\n811\n812\n813\n814\n815\n816\n817\n818\n819\n820\n821\n822\n823\n824\n825\n826\n827\n828\n829\n830\n831\n832\n833\n834\n835\n836\n837\n838\n839\n840\n841\n842\n843\n844\n845\n846\n847\n848\n849\n850\n851\n852\n853\n854\n855\n856\n857\n858\n859\n860\n861\n862\n863\n864\n865\n866\n867\n868\n869\n870\n871\n872\n873\n874\n875\n876\n877\n878\n879\n880\n881\n882\n883\n884\n885\n886\n887\n888\n889\n890\n891\n892\n893\n894\n895\n896\n897\n898\n899\n900\n901\n902\n903\n904\n905\n906\n907\n908\n909\n910\n911\n912\n913\n914\n915\n916\n917\n918\n919\n920\n921\n922\n923\n924\n925\n926\n927\n928\n929\n930\n931\n932\n933\n934\n935\n936\n937\n938\n939\n940\n941\n942\n943\n944\n945\n946\n947\n948\n949\n950\n951\n952\n953\n954\n955\n956\n957\n958\n959\n960\n961\n962\n963\n964\n965\n966\n967\n968\n969\n970\n971\n972\n973\n974\n975\n976\n977\n978\n979\n980\n981\n982\n983\n984\n985\n986\n987\n988\n989\n990\n991\n992\n993\n994\n995\n996\n997\n998\n*E\n*E\n SUBMITTED BY: [e107947]     107947    09/05/22 02:40:02\r\n";
           //submitResult = "21221\n1\n2\n3\n*E\n*E\n SUBMITTED BY: [e21221]     23748    06/06/22 07:57:40\r\n";
            //CommonClass.SaveFormatBidReceipt(submitResult);
            if (GlobalSettings.WbidUserContent.UserInformation.Domicile == null)
                GlobalSettings.WbidUserContent.UserInformation.Domicile = "BWI";
            ScreenSizeManagement();
            NSNotificationCenter defaultCenter = NSNotificationCenter.DefaultCenter;
            defaultCenter.AddObserver(this, modeSelector, themeKeyString, null);//.net 8 changes
            reloadNotification = NSNotificationCenter.DefaultCenter.AddObserver((NSString)"reloadModernViewFromMainClass", (NSNotification notification) =>
            {
                if (CommonClass.ModernController != null)
                {
                    CommonClass.ModernController.ReloadContent();
                    HandleBlueShadowButton();
                }
            });
//            btnMIL.Enabled = true;
//            GlobalSettings.WBidINIContent.User.MIL = false;
//            GlobalSettings.MenuBarButtonStatus.IsMIL = false;


            //this.Window.SetFrame (new  CGRect (0, 0, 550, 550),true);
            //this.Window.IsZoomed = false;
            this.ShouldCascadeWindows = false;
            isNeedToClose = 0;
            GlobalSettings.RedoStack = new List<WBidState> ();
            GlobalSettings.UndoStack = new List<WBidState> ();
            try {
                btnRedo.Activated += (sender, e) => {
                    RedoOperation ();
                };
                btnUndo.Activated += (sender, e) => {
                    UndoOperation ();
                };
                closeButton = this.Window.StandardWindowButton (NSWindowButton.CloseButton);
                closeButton.Activated += (sender, e) => {
                    SetScreenSize();

                    if (GlobalSettings.isModified) {
                        
                        var alert = new NSAlert ();
                        alert.Window.Title = "WBidMax";
                        alert.MessageText = "Save your Changes?";
                        //alert.InformativeText = "There are no Latest News available..!";
                        Console.WriteLine("Position" + this.Window.Frame);

                        int saveindex = 0;
                        if (GlobalSettings.WBidINIContent.User.SmartSynch) {
                            alert.AddButton ("Save & Synch");
                            saveindex = 1;
                            alert.Buttons [0].Activated += delegate {
                                StateManagement stateManagement = new StateManagement ();
                                stateManagement.UpdateWBidStateContent ();
                                GlobalSettings.WBidStateCollection.IsModified = true;
                                WBidHelper.SaveStateFile (WBidHelper.WBidStateFilePath);
                                alert.Window.Close ();
                                NSApplication.SharedApplication.StopModal ();
                                isNeedToClose = 2;
                                Synch();
                                //SynchState ();
                            };
                        }

                        alert.AddButton ("Save & Exit");
                        alert.AddButton ("Exit");
                        alert.AddButton ("Cancel");
                        alert.Buttons [saveindex].Activated += delegate {
                            // save and exit
                            StateManagement stateManagement = new StateManagement ();
                            stateManagement.UpdateWBidStateContent ();
                            if (GlobalSettings.isModified) {
                                GlobalSettings.WBidStateCollection.IsModified = true;
                                WBidHelper.SaveStateFile (WBidHelper.WBidStateFilePath);
                            }
                            //ExitApp ();
                            alert.Window.Close ();
                            NSApplication.SharedApplication.StopModal ();
                            isNeedToClose = 2;
                            CheckSmartSync ();
                        };
                        alert.Buttons [saveindex + 1].Activated += delegate {
                            ExitApp ();    
                            alert.Window.Close ();
                            NSApplication.SharedApplication.StopModal ();
                        };
                        alert.RunModal ();


                    } else {
                        //ExitApp ();
                        SetScreenSize();

                        isNeedToClose = 2;
                        CheckSmartSync ();


                    }

                };
                this.Window.Title = WBidCollection.SetTitile ();
                SetPropertyNames ();
                setViews ();
                CommonClass.AppDelegate.ReloadMenu ();
                txtGoToLine.Activated += HandleGoToLine;
                txtGoToLine.Changed += delegate {            
                    txtGoToLine.StringValue = txtGoToLine.StringValue;
                };
                SetupAdminView ();
                SetupPrintOptionView ();
                HandleBlueShadowButton();


                wBIdStateContent = GlobalSettings.WBidStateCollection.StateList.FirstOrDefault (x => x.StateName == GlobalSettings.WBidStateCollection.DefaultName);
                UpdateSaveButton (false);

                SetVacButtonStates ();

//                if (btnEOM.State == NSCellStateValue.On || btnVacation.State == NSCellStateValue.On || btnDrop.State == NSCellStateValue.On) {
//                    BeginInvokeOnMainThread (() => {
//                        applyVacation ();
//                    });
//                } else if (btnOverlap.State == NSCellStateValue.On) {
//                    applyOverLapCorrection ();
//                }



                UpdateUndoRedoButtons ();
                if (GlobalSettings.WBidINIContent.User.AutoSave) {
                    AutoSave ();
                }
                BeginInvokeOnMainThread (() => {
                    FirstTime = true;
                    Synch ();
                });
                TextColor(btnVacation, NSColor.Black);
                SetFlightDataDiffButton();
                

            } catch (Exception ex) {
                CommonClass.AppDelegate.ErrorLog (ex);
                CommonClass.AppDelegate.ShowErrorMessage (WBidErrorMessages.CommonError);
            }
        }
        private bool IsCommuteAutoAvailable()
        {
            bool isCommuteAutoAvailable = false;
            if (wBIdStateContent.CxWtState.CLAuto.Cx || wBIdStateContent.CxWtState.CLAuto.Wt || (wBIdStateContent.SortDetails.BlokSort.Contains("33") || wBIdStateContent.SortDetails.BlokSort.Contains("34") || wBIdStateContent.SortDetails.BlokSort.Contains("35")))
            {
                isCommuteAutoAvailable = true;
            }
            return isCommuteAutoAvailable;
        }
        public void SetFlightDataDiffButton()
        {
            bool IsEnableFltDiff;
            if (GlobalSettings.IsNeedToEnableVacDiffButton)
            {
                bool isCommuteAutoAvailable = false;
                if (wBIdStateContent != null && (wBIdStateContent.CxWtState.CLAuto.Cx || wBIdStateContent.CxWtState.CLAuto.Wt || (wBIdStateContent.SortDetails.BlokSort.Contains("33") || wBIdStateContent.SortDetails.BlokSort.Contains("34") || wBIdStateContent.SortDetails.BlokSort.Contains("35"))))
                {
                    isCommuteAutoAvailable = true;
                }


                if (GlobalSettings.CurrentBidDetails != null && GlobalSettings.CurrentBidDetails.Postion == "FA")
                {
                    //For FA, the Flt difference button should be display only when user set any commutable line auto in constraints ,weights,sorts or in bid auto
                    IsEnableFltDiff = isCommuteAutoAvailable && (GlobalSettings.ServerFlightDataVersion != GlobalSettings.WBidINIContent.LocalFlightDataVersion);
                }
                else
                {
                    IsEnableFltDiff = ((isCommuteAutoAvailable && (GlobalSettings.ServerFlightDataVersion != GlobalSettings.WBidINIContent.LocalFlightDataVersion)) || (btnVacation.Enabled || btnEOM.Enabled));
                }
            }
            else
            {
                IsEnableFltDiff = false;
            }
            btnVacDiff.Hidden = !IsEnableFltDiff;
        }
        private void setVacationButton()
        { }
        partial void funBlueShadeFunctionality(Foundation.NSObject sender)
        {
            foreach (var lines in GlobalSettings.Lines)
            {
                lines.ManualScroll = 0;
            }
            UpdateSaveButton(true);
            NSNotificationCenter.DefaultCenter.PostNotificationName ("reloadModernViewFromMainClass", null);
        }
        void ScreenSizeManagement()
        {

            if (GlobalSettings.WBidINIContent.MainWindowSize.IsMaximised == true) {
                this.Window.IsZoomed = true;
            } else {
                if (GlobalSettings.WBidINIContent.MainWindowSize.Height > 0) {
                    CGRect ScreenFrame = new CGRect (GlobalSettings.WBidINIContent.MainWindowSize.Left, GlobalSettings.WBidINIContent.MainWindowSize.Top, GlobalSettings.WBidINIContent.MainWindowSize.Width, GlobalSettings.WBidINIContent.MainWindowSize.Height);
                    this.Window.SetFrame (ScreenFrame, true);

                } else {
                    
                    SetScreenSize ();
                }
            }
        }

        void SetScreenSize()
        {
            GlobalSettings.WBidINIContent.MainWindowSize.Left =(int) this.Window.Frame.X;
            GlobalSettings.WBidINIContent.MainWindowSize.Top = (int)this.Window.Frame.Y;
            GlobalSettings.WBidINIContent.MainWindowSize.Width = (int)this.Window.Frame.Width;
            GlobalSettings.WBidINIContent.MainWindowSize.Height = (int)this.Window.Frame.Height;
            GlobalSettings.WBidINIContent.MainWindowSize.IsMaximised = this.Window.IsZoomed;    
            //save the state of the INI File
            WBidHelper.SaveINIFile (GlobalSettings.WBidINIContent, WBidHelper.GetWBidINIFilePath ());
        }


        void SetupPrintOptionView ()
        {
            txtLineNoPrint.Enabled = (btnLinesPrint.SelectedTag == 0);
            btnLinesPrint.Activated += delegate {
                txtLineNoPrint.Enabled = (btnLinesPrint.SelectedTag == 0);
            };
            txtLineNoPrint.Changed += delegate {
                txtLineNoPrint.StringValue = txtLineNoPrint.StringValue;
            };
            btnPrintCancel.Activated += delegate {
                NSApplication.SharedApplication.EndSheet (CommonClass.Panel);
                CommonClass.Panel.OrderOut (this);
            };
            btnPrintOK.Activated += delegate {
                NSApplication.SharedApplication.EndSheet (CommonClass.Panel);
                CommonClass.Panel.OrderOut (this);
                var printContent = string.Empty;

                if (btnLinesPrint.SelectedTag == 0) {
                    if (txtLineNoPrint.StringValue == "")
                        return;
                    int count = 0;
                    foreach (var line in GlobalSettings.Lines) {
                        printContent += CommonClass.PrintBidLines (line.LineNum);
                        count++;
                        if (count == int.Parse (txtLineNoPrint.StringValue))
                            break;
                    }
                } else {
                    foreach (var line in GlobalSettings.Lines) {
                        printContent += CommonClass.PrintBidLines (line.LineNum);
                    }
                }
                var inv = new InvisibleWindowController ();
                CommonClass.MainController.Window.AddChildWindow (inv.Window, NSWindowOrderingMode.Below);
                var txt = new NSTextView (new CGRect (0, 0, 550, 550));
                txt.Font = NSFont.FromFontName ("Courier", 6);
                inv.Window.ContentView.AddSubview (txt);
                txt.Value = printContent;
                var pr = NSPrintInfo.SharedPrintInfo;
                pr.VerticallyCentered = false;
                pr.TopMargin = 2.0f;
                pr.BottomMargin = 2.0f;
                pr.LeftMargin = 1.0f;
                pr.RightMargin = 1.0f;
                txt.Print (this);
                inv.Close ();

            };
        }

        void ExitApp ()
        {
            if (cswWC != null)
                cswWC.Window.Close();

            if (qsWC != null)
                qsWC.Window.Close();


            if (baWC != null)
                baWC.Window.Close();

            if (pairing != null)
                pairing.Window.Close();
            
            CloseAllChildWindows ();
            this.Window.Close ();
            this.Window.OrderOut (this);
        }

        public void UpdateAutoSave ()
        {
            
            if (GlobalSettings.WBidINIContent.User.AutoSave) {
                if (timer != null)
                    timer.Stop ();
                AutoSave ();
            } else {
                if (timer != null)
                    timer.Stop ();
            }
        }
        
        /// <summary>
        /// This will save the current bid state automatically dependes on the Settings in the Configuration=>user tab
        /// </summary>
        public void AutoSave ()
        {
            if (GlobalSettings.WBidINIContent.User.AutoSave) {
                timer = new System.Timers.Timer (GlobalSettings.WBidINIContent.User.AutoSavevalue * 60000) {
                    Interval = GlobalSettings.WBidINIContent.User.AutoSavevalue * 60000,
                    Enabled = true
                };
                timer.Elapsed += timer_Elapsed;
            }
        }

        private void timer_Elapsed (object sender, System.Timers.ElapsedEventArgs e)
        {
            StateManagement stateManagement = new StateManagement ();
            stateManagement.UpdateWBidStateContent ();
            GlobalSettings.WBidStateCollection.IsModified = true;
            WBidHelper.SaveStateFile (WBidHelper.WBidStateFilePath);
            //save the state of the INI File
            WBidHelper.SaveINIFile (GlobalSettings.WBidINIContent, WBidHelper.GetWBidINIFilePath ());
            GlobalSettings.isModified = false;
            InvokeOnMainThread (() => {
                UpdateSaveButton (false);
                UpdateUndoRedoButtons ();
            });
        }


        //public override void TraitCollectionDidChange(UITraitCollection previousTraitCollection)
        //{
        //    base.TraitCollectionDidChange(previousTraitCollection);
        //    // Mode changed, do something.
        //}

       

        private void ShowMessageBox (string title, string content)
        {
            var alert = new NSAlert ();
            alert.MessageText = title;
            alert.InformativeText = content;
            alert.RunModal ();
        }

        static MILData CreateNewMILFile ()
        {
            MILData milData;
            CalculateMIL calculateMIL = new CalculateMIL ();
            MILParams milParams = new MILParams ();
            NetworkData networkData = new NetworkData ();
            if (System.IO.File.Exists (WBidHelper.GetAppDataPath () + "/FlightData.NDA"))
                networkData.ReadFlightRoutes ();
            else
                networkData.GetFlightRoutes ();
            //calculate MIL value and create MIL File
            //==============================================
            WBidCollection.GenerateSplitPointCities ();
            milParams.Lines = GlobalSettings.Lines.ToList ();
            Dictionary<string, TripMultiMILData> milvalue = calculateMIL.CalculateMILValues (milParams);
            milData = new MILData ();
            milData.Version = GlobalSettings.MILFileVersion;
            milData.MILValue = milvalue;
            var stream = File.Create (WBidHelper.MILFilePath);
            ProtoSerailizer.SerializeObject (WBidHelper.MILFilePath, milData, stream);
            stream.Dispose ();
            stream.Close ();
            return milData;
        }

        private void SetMILDataAfterSynch ()
        {
            var wBidStateContent = GlobalSettings.WBidStateCollection.StateList.FirstOrDefault (x => x.StateName == GlobalSettings.WBidStateCollection.DefaultName);

            var MILDates = wBidStateContent.MILDateList;

            if (MILDates.Count > 0) {
                isNeedtoCreateMILFile = false;
                if (GlobalSettings.MILDates == null || MILDates.Count != GlobalSettings.MILDates.Count)
                    isNeedtoCreateMILFile = true;
                else {
                    for (int count = 0; count < MILDates.Count; count++) {
                        if (GlobalSettings.MILDates [count].StartAbsenceDate != MILDates [count].StartAbsenceDate || GlobalSettings.MILDates [count].EndAbsenceDate != MILDates [count].EndAbsenceDate) {
                            isNeedtoCreateMILFile = true;
                            break;
                        }

                    }
                }
                GlobalSettings.MILDates = GenarateOrderedMILDates (wBidStateContent.MILDateList);
                MILData milData;
                InvokeOnMainThread (() => {
                    overlay.UpdateText ("Calculating MIL");
                });

                //InvokeInBackground (() => {
                if (System.IO.File.Exists (WBidHelper.MILFilePath) && !isNeedtoCreateMILFile) {
                    using (FileStream milStream = File.OpenRead (WBidHelper.MILFilePath)) {

                        MILData milDataobject = new MILData ();
                        milData = ProtoSerailizer.DeSerializeObject (WBidHelper.MILFilePath, milDataobject, milStream);

                    }
                } else {
                    overlayPanel.SetContentSize (new CGSize (400, 120));
                    overlay = new OverlayViewController ();
                    overlay.OverlayText = "Calculating MIL \n Please wait..";
                    overlayPanel.ContentView = overlay.View;

                    milData = CreateNewMILFile ();




                }


                //Apply MIL values (calculate property values including Modern bid line properties
                //==============================================

                GlobalSettings.MILData = milData.MILValue;
                GlobalSettings.MenuBarButtonStatus.IsMIL = true;

                RecalcalculateLineProperties recalcalculateLineProperties = new RecalcalculateLineProperties ();
                recalcalculateLineProperties.CalcalculateLineProperties ();

//                InvokeOnMainThread (() => {
//                    GlobalSettings.isModified = true;
//                    CommonClass.lineVC.UpdateSaveButton ();
//                    syncOverlay.Hide ();
//                    CommonClass.lineVC.SetVacButtonStates ();
//                    NSNotificationCenter.DefaultCenter.PostNotificationName ("DataReload", null);
//                    this.DismissViewController (true, null);
//                });




                //});

            }
        }

        private void SetEOMVacationDataAfterSynch ()
        {
            try {
                var wBidStateContent = GlobalSettings.WBidStateCollection.StateList.FirstOrDefault (x => x.StateName == GlobalSettings.WBidStateCollection.DefaultName);

                if (GlobalSettings.CurrentBidDetails.Postion == "FA") {
                    if (GlobalSettings.FAEOMStartDate != null && GlobalSettings.FAEOMStartDate != DateTime.MinValue) {

                        overlay.UpdateText ("Calculating EOM");
                        BeginInvokeOnMainThread (() => {
                            CreateEOMVacforFA ();
                        });
                    }
                } else {
                    string currentBidName = WBidHelper.GenerateFileNameUsingCurrentBidDetails ();

                    //string zipFileName = GenarateZipFileName();
                    string vACFileName = WBidHelper.GetAppDataPath () + "//" + currentBidName + ".VAC";
                    //Cheks the VAC file exists
                    bool vacFileExists = File.Exists (vACFileName);

                    if (!vacFileExists) {
                        InvokeOnMainThread (() => {

                            ShowMessageBox ("Smart Sync", "Previous state had EOM selected and we are downloading Vacation Data");
                            overlay.UpdateText ("Calculating EOM");
                        });


                        //InvokeOnMainThread (() => {

                        CreateEOMVacationforCP ();


                        //});
                    } else {

                        InvokeOnMainThread (() => {
                            overlay.UpdateText ("Calculating EOM");

                            if (GlobalSettings.VacationData == null) {
                                using (FileStream vacstream = File.OpenRead (vACFileName)) {

                                    Dictionary<string, TripMultiVacData> objineinfo = new Dictionary<string, TripMultiVacData> ();
                                    GlobalSettings.VacationData = ProtoSerailizer.DeSerializeObject (vACFileName, objineinfo, vacstream);
                                }
                            }


                        });
                    }
                }
            } catch (Exception ex) {
                wBIdStateContent.MenuBarButtonState.IsEOM = false;
                throw ex;
            }


        }

        private List<Absense> GenarateOrderedMILDates (List<Absense> milList)
        {
            List<Absense> absence = new List<Absense> ();
            if (milList.Count > 0) {
                absence.Add (new Absense {
                    StartAbsenceDate = milList.FirstOrDefault ().StartAbsenceDate,
                    EndAbsenceDate = milList.FirstOrDefault ().EndAbsenceDate,
                    AbsenceType = "VA"
                });

                for (int count = 0; count < milList.Count - 1; count++) {
                    if ((milList [count + 1].StartAbsenceDate - milList [count].EndAbsenceDate).Days == 1) {
                        absence [absence.Count - 1].EndAbsenceDate = milList [count + 1].EndAbsenceDate;
                    } else {
                        absence.Add (new Absense {
                            StartAbsenceDate = milList [count + 1].StartAbsenceDate,
                            EndAbsenceDate = milList [count + 1].EndAbsenceDate,
                            AbsenceType = "VA"
                        });
                    }
                }
            }
            return absence;
        }

        public bool IsSynchStart;

        public int SynchStateVersion { get; set; }
        public int SynchQSVersion { get; set; }

        public DateTime ServerSynchTime { get; set; }

        private void CheckSmartSync ()
        {
            if (GlobalSettings.WBidStateCollection != null && GlobalSettings.WBidINIContent != null && GlobalSettings.WBidINIContent.User.SmartSynch) {

                var alert = new NSAlert ();
                alert.Window.Title = "Smart Sync";
                alert.MessageText = "Do you want to sync local changes with Server?";
                //alert.InformativeText = "There are no Latest News available..!";
                alert.AddButton ("Yes");
                alert.AddButton ("No");
                //alert.AddButton ("Cancel");
                alert.Buttons [1].Activated += delegate {
                    alert.Window.Close ();
                    NSApplication.SharedApplication.StopModal ();
                    if (isNeedToClose == 1)
                        GoToHomeWindow ();
                    else if (isNeedToClose == 2)
                        
                        ExitApp ();                
                };
                alert.Buttons [0].Activated += delegate {
                    alert.Window.Close ();
                    NSApplication.SharedApplication.StopModal ();
                    //isNeedToClose=true;
                    Synch();
                    //SynchState ();
                };
                alert.RunModal ();
            } else {
                if (isNeedToClose == 1)
                    GoToHomeWindow ();
                else if (isNeedToClose == 2)
                    ExitApp ();                
            }
        }

        private void Synch ()
        {
            if (GlobalSettings.WBidStateCollection != null && GlobalSettings.WBidINIContent != null && GlobalSettings.WBidINIContent.User.SmartSynch) {
                overlayPanel = new NSPanel ();
                overlayPanel.SetContentSize (new CGSize (400, 120));
                overlay = new OverlayViewController ();
                overlay.OverlayText = "Smart Synchronisation checking server version..\n Please wait..";
                overlayPanel.ContentView = overlay.View;
                NSApplication.SharedApplication.BeginSheet (overlayPanel, this.Window);

                BeginInvokeOnMainThread (() => {
                    SynchStateForApplicationLoad ();
                });
            }
        }

        private void SynchStateForApplicationLoad ()
        {

            try {
                // MessageBoxResult msgResult;
                bool isConnectionAvailable = Reachability.CheckVPSAvailable();

                if (isConnectionAvailable) {

                    IsSynchStart = true;
                    string stateFileName = WBidHelper.GenerateFileNameUsingCurrentBidDetails ();
                    SynchStateVersion = int.Parse (GlobalSettings.WBidStateCollection.SyncVersion);
                    if (GlobalSettings.QuickSets == null)
                    {
                        if (File.Exists(WBidHelper.GetQuickSetFilePath()))
                        {
                            GlobalSettings.QuickSets = XmlHelper.DeserializeFromXml<QuickSets>(WBidHelper.GetQuickSetFilePath());
                        }
                        else{
                            GlobalSettings.QuickSets  = new QuickSets();
                            GlobalSettings.QuickSets.QuickSetColumn = new List<QuickSetColumn>();
                            GlobalSettings.QuickSets.QuickSetCSW = new List<QuickSetCSW>();

                        }


                    }
                    SynchQSVersion = (GlobalSettings.QuickSets != null && GlobalSettings.QuickSets.SyncQuickSetVersion != null) ? int.Parse(GlobalSettings.QuickSets.SyncQuickSetVersion) : 0;

                    WBGetStateDTO wbStateDTO = new WBGetStateDTO();
                    wbStateDTO.Employeeumber = int.Parse(GlobalSettings.WbidUserContent.UserInformation.EmpNo);
                    wbStateDTO.QuickSetFileName = GlobalSettings.WbidUserContent.UserInformation.EmpNo;
                    wbStateDTO.StateName = stateFileName;
                    wbStateDTO.Year = GlobalSettings.CurrentBidDetails.Year;
                    wbStateDTO.FileType = 2;


                    WBStateInfoDTO versionInfo = GetWBServerStateandquicksetVersionNumber(wbStateDTO);

                    //Get server State Version
                    //VersionInfo versionInfo = GetServerVersion (stateFileName);
                    //syncOverlay.Hide();
                    NSApplication.SharedApplication.EndSheet (overlayPanel);
                    overlayPanel.OrderOut (this);

                    if (versionInfo != null) {
                        ServerSynchTime = DateTime.Parse (versionInfo.StateLastUpdatedDate, CultureInfo.InvariantCulture);

                        if (versionInfo.StateVersionNumber != string.Empty || versionInfo.QuickSetVersionNumber != string.Empty){
                            int serverVersion = Convert.ToInt32(versionInfo.StateVersionNumber);
                            int serverQSversion = Convert.ToInt32(versionInfo.QuickSetVersionNumber);

                            if (SynchStateVersion != serverVersion || GlobalSettings.WBidStateCollection.IsModified || SynchQSVersion != serverQSversion || GlobalSettings.QuickSets.IsModified){
                                //conflict
                                

                                //confNotif = NSNotificationCenter.DefaultCenter.AddObserver ((NSString)"SyncConflict", (NSNotification notification) => {
                                //  string str = notification.Object.ToString ();
                                //  NSNotificationCenter.DefaultCenter.RemoveObserver (confNotif);
                                //  BeginInvokeOnMainThread (() => {
                                //      if (str == "server") {
                                //          FirstTime = true;
                                //          overlayPanel = new NSPanel ();
                                //          overlayPanel.SetContentSize (new CGSize (400, 120));
                                //          overlay = new OverlayViewController ();
                                //          overlay.OverlayText = "Synching current State FROM server \n Please wait..";
                                //          overlayPanel.ContentView = overlay.View;
                                //          NSApplication.SharedApplication.BeginSheet (overlayPanel, this.Window);

                                //          WBidHelper.PushToUndoStack ();
                                //          UpdateUndoRedoButtons ();
                                //          BeginInvokeOnMainThread (() => {
                                //              GetStateFromServer (stateFileName);
                                //          });
                                //      } else if (str == "local") {
                                //          FirstTime = true;
                                //          overlayPanel = new NSPanel ();
                                //          overlayPanel.SetContentSize (new CGSize (400, 120));
                                //          overlay = new OverlayViewController ();
                                //          overlay.OverlayText = "Synching current State TO server \n Please wait..";
                                //          overlayPanel.ContentView = overlay.View;
                                //          NSApplication.SharedApplication.BeginSheet (overlayPanel, this.Window);

                                //          BeginInvokeOnMainThread (() => {
                                //              UploadLocalVersionToServer (stateFileName);
                                //          });
                                //      } else {
                                //          IsSynchStart = false;
                                //      }                           

                                //  });
                                //});

                                InvokeOnMainThread (() => {
                                    //var panel = new NSPanel();
                                    //var synchConf = new SynchConflictViewController();
                                    //synchConf.serverSynchTime = ServerSynchTime;
                                    //if (serverVersion == 0)
                                    //    synchConf.noServer = true;
                                    //CommonClass.Panel = panel;
                                    //panel.SetContentSize(new CGSize(450, 285));
                                    //panel.ContentView = synchConf.View;
                                    //NSApplication.SharedApplication.BeginSheet(panel, CommonClass.MainController.Window);
                                    ////NSApplication.SharedApplication.RunModalForWindow (panel);



                                    // SynchView Initiating   added by Francis 21 Jul 2020

                                   
                                        if (synchView == null)
                                            synchView = new SynchViewWindowController();

                                   //SynchSelectionController synchConf = new SynchSelectionController();



                                    //pass  versionInfo 
                                    string serverupdated = DateTime.Now.ToString();
                                    string localupdated = DateTime.Now.AddDays(-1).ToString();



                                    // SynchSelection Observer actions
                                    synchView.LocalStateSynchTime = GlobalSettings.WBidStateCollection.StateUpdatedTime;
                                    synchView.LocalQSSynchTime = (GlobalSettings.QuickSets == null) ? DateTime.MinValue : GlobalSettings.QuickSets.QuickSetUpdatedTime;

                                    synchView.ServerStateSynchTime = versionInfo.StateLastUpdate; ;
                                    synchView.ServerQSSynchTime = versionInfo.QuickSetLastUpdated;

                                    if (!GlobalSettings.WBidINIContent.User.IsSynchViewFloat)
                                    {

                                        // close window notification
                                        notifClos = NSNotificationCenter.DefaultCenter.AddObserver((NSString)"closeSynch", (NSNotification n) =>
                                        {
                                            NSNotificationCenter.DefaultCenter.RemoveObserver(notifClos);
                                            if (synchView != null)
                                            {
                                                synchView.CloseSynch();
                                                synchView = null;
                                            }


                                        });


                                        synchView.ShowWindow(this);
                                        

                                        // notification for synch processing
                                        synchNotif = NSNotificationCenter.DefaultCenter.AddObserver(new Foundation.NSString("synchProcessing"), async (NSNotification notification) =>
                                        {


                                            string selectionString = notification.Object.ToString();

                                            NSNotificationCenter.DefaultCenter.RemoveObserver(synchNotif);

                                            if (synchView != null && selectionString != "000")
                                            {
                                                synchView.CloseSynch();
                                                synchView = null;
                                            }

                                            // selectionString = 300 means selectionindex = 3,StateSegement = 0,QuickSegment = 0 ()
                                            // selectionString = 311 means selectionindex = 3,StateSegement = 1,QuickSegment = 1
                                            // selectionString = 301 means selectionindex = 3,StateSegement = 0,QuickSegment = 1
                                            // selectionString = 310 means selectionindex = 3,StateSegement = 1,QuickSegment = 0
                                            // selectionString = 100 means selectionindex = 1,StateSegement = 0
                                            // selectionString = 110 means selectionindex = 1,StateSegement = 1
                                            // selectionString = 200 means selectionindex = 2,QuickSegment = 0
                                            // selectionString = 201 means selectionindex = 2,QuickSegment = 1

                                            switch (selectionString)
                                            {
                                                case "100":
                                                    DataSynchSelecedValue = 1;
                                                    IsStateFromServer = false;
                                                    IskeepLocalState = true;
                                                    IsQSFromServer = false;
                                                    IskeepLocalQS = false;
                                                    break;
                                                case "110":
                                                    DataSynchSelecedValue = 1;
                                                    IsStateFromServer = true;
                                                    IskeepLocalState = false;
                                                    IsQSFromServer = false;
                                                    IskeepLocalQS = false;
                                                    break;
                                                case "200":
                                                    DataSynchSelecedValue = 2;
                                                    IsStateFromServer = false;
                                                    IskeepLocalState = false;
                                                    IsQSFromServer = false;
                                                    IskeepLocalQS = true;
                                                    break;
                                                case "201":
                                                    DataSynchSelecedValue = 2;
                                                    IsStateFromServer = false;
                                                    IskeepLocalState = false;
                                                    IsQSFromServer = true;
                                                    IskeepLocalQS = false;
                                                    break;
                                                case "300":
                                                    DataSynchSelecedValue = 3;
                                                    IsStateFromServer = false;
                                                    IskeepLocalState = true;
                                                    IsQSFromServer = false;
                                                    IskeepLocalQS = true;
                                                    break;
                                                case "301":
                                                    DataSynchSelecedValue = 3;
                                                    IsStateFromServer = false;
                                                    IskeepLocalState = true;
                                                    IsQSFromServer = true;
                                                    IskeepLocalQS = false;
                                                    break;
                                                case "310":
                                                    DataSynchSelecedValue = 3;
                                                    IsStateFromServer = true;
                                                    IskeepLocalState = false;
                                                    IsQSFromServer = false;
                                                    IskeepLocalQS = true;
                                                    break;
                                                case "311":
                                                    DataSynchSelecedValue = 3;
                                                    IsStateFromServer = true;
                                                    IskeepLocalState = false;
                                                    IsQSFromServer = true;
                                                    IskeepLocalQS = false;
                                                    break;
                                            }
                                            //===========================================
                                            // Selected only for state file sync
                                            if (DataSynchSelecedValue == 1)
                                            {
                                                if (SynchStateVersion != serverVersion || GlobalSettings.WBidStateCollection.IsModified)
                                                {
                                                    wbStateDTO.FileType = 0;
                                                    if (IsStateFromServer) // Get Server state to local
                                                    {
                                                        FirstTime = true;
                                                        SetOverlay("Synching current State FROM server \n Please wait..");
                                                        WBidHelper.PushToUndoStack();
                                                        UpdateUndoRedoButtons();

                                                        bool status = await Task.Run(() => GetWBStateAndquicksetFromServer(wbStateDTO));

                                                        if (status)
                                                        {
                                                            InvokeOnMainThread(() =>
                                                            {
                                                                ShowMessageBox("Smart Sync", "Successfully Synchronized  your State file from server.");
                                                                //syncOverlay.Hide();
                                                                overlayPanel.OrderOut(this);
                                                            });
                                                        }
                                                        else
                                                        {
                                                            InvokeOnMainThread(() =>
                                                            {
                                                                ShowMessageBox("Smart Sync", "An error occured while synchronizing your state to the server.  You can work on this bid, but your state file for this bid may become unsynchronized from a previous state.");
                                                                //syncOverlay.Hide();
                                                                overlayPanel.OrderOut(this);
                                                            });
                                                        }

                                                    }
                                                    else if (IskeepLocalState) //Save local State to server
                                                    {
                                                        FirstTime = true;
                                                        bool result = false;
                                                        SetOverlay("Synching current State TO server \n Please wait..");
                                                        //syncOverlay.updateLoadingText("Synching current State TO server \n Please wait..");
                                                        //syncOverlay.Show();


                                                        result = await Task.Run(() => SaveWBStateAndQuickSetToServer(stateFileName));

                                                        if (result)
                                                        {
                                                            InvokeOnMainThread(() =>
                                                            {
                                                                ShowMessageBox("Smart Sync", "Successfully Synchronized  your State file with the server.");
                                                                //syncOverlay.Hide();
                                                                overlayPanel.OrderOut(this);
                                                                if (FirstTime)
                                                                    NSNotificationCenter.DefaultCenter.PostNotificationName("DataReload", null);//loadSummaryListAndHeader();

                                                                FirstTime = false;
                                                            });
                                                        }
                                                        else
                                                        {
                                                            InvokeOnMainThread(() =>
                                                            {
                                                                ShowMessageBox("Smart Sync", "An error occured while synchronizing your state to the server.  You can work on this bid, but your state file for this bid may become unsynchronized from a previous state.");
                                                                //syncOverlay.Hide();
                                                                overlayPanel.OrderOut(this);
                                                            });
                                                        }
                                                    }
                                                }
                                                else
                                                {
                                                    InvokeOnMainThread(() =>
                                                    {
                                                        ShowMessageBox("Smart Sync", "Your State file is already synchronized with the server");
                                                        //syncOverlay.Hide();
                                                        overlayPanel.OrderOut(this);
                                                    });
                                                }
                                            }
                                            else if (DataSynchSelecedValue == 2) // Selected only for Quicksets  sync
                                            {
                                                if (SynchQSVersion != serverQSversion || GlobalSettings.QuickSets.IsModified)
                                                {
                                                    wbStateDTO.FileType = 1;
                                                    if (IsQSFromServer)
                                                    {
                                                        SetOverlay("Synching Quicksets From server \n Please wait..");WBidHelper.PushToUndoStack();
                                                        UpdateUndoRedoButtons();
                                                        //syncOverlay = new LoadingOverlay(View.Bounds, "Synching Quicksets From server \n Please wait..");
                                                       // View.Add(syncOverlay);
                                                        bool result = await Task.Run(() => GetWBStateAndquicksetFromServer(wbStateDTO));
                                                        if (result)
                                                        {
                                                            InvokeOnMainThread(() =>
                                                            {
                                                                ShowMessageBox("Smart Sync", "Your Quickset is successfully synchronized with server.");
                                                                //syncOverlay.Hide();
                                                                overlayPanel.OrderOut(this);
                                                            });
                                                        }
                                                    }
                                                    else if (IskeepLocalQS)
                                                    {
                                                        SetOverlay("Synching Quikcsets TO server \n Please wait..");
                                                       // syncOverlay = new LoadingOverlay(View.Bounds, "Synching Quikcsets TO server \n Please wait..");
                                                       // View.Add(syncOverlay);
                                                        bool result = await Task.Run(() => SaveWBStateAndQuickSetToServer(stateFileName));
                                                        if (result)
                                                        {
                                                            InvokeOnMainThread(() =>
                                                            {
                                                                ShowMessageBox("Smart Sync", "Your Quickset is successfully synchronized with server.");
                                                                //syncOverlay.Hide();
                                                                overlayPanel.OrderOut(this);
                                                            });
                                                        }
                                                    }
                                                }
                                                else
                                                {
                                                    InvokeOnMainThread(() =>
                                                    {
                                                        ShowMessageBox("Smart Sync", "Your Quickset is already synchronized with the server");
                                                       // syncOverlay.Hide();
                                                        overlayPanel.OrderOut(this);
                                                    });
                                                }
                                            }
                                            else if (DataSynchSelecedValue == 3)// bOth State and Quickset sync
                                            {
                                                wbStateDTO.FileType = 2;
                                                bool Stateresult = false;
                                                bool QSresult = false;
                                                SetOverlay("Synching Quikcsets and States \n Please wait..");

                                                //syncOverlay = new LoadingOverlay(View.Bounds, "Synching Quicksets and States  \n Please wait..");
                                               // View.Add(syncOverlay);
                                                if (IsStateFromServer || IsQSFromServer)
                                                {
                                                    Stateresult = await Task.Run(() => GetWBStateAndquicksetFromServer(wbStateDTO));
                                                }
                                                if (IskeepLocalState || IskeepLocalQS)
                                                {
                                                    QSresult = await Task.Run(() => SaveWBStateAndQuickSetToServer(stateFileName));
                                                }
                                                string alert = string.Empty;
                                                if (QSresult || Stateresult)
                                                {
                                                    alert = "Your Quickset and State  is Successsfully synchronized with server";
                                                    if (FirstTime)
                                                        NSNotificationCenter.DefaultCenter.PostNotificationName("DataReload", null);//loadSummaryListAndHeader();

                                                    FirstTime = false;
                                                }

                                                ShowMessageBox("Smart Sync", alert);
                                                //syncOverlay.Hide();
                                                overlayPanel.OrderOut (this);
                                            }
                                            //==========================================
                                        });
                                    }

                                    else
                                        synchView.Close();
                                    
                                });


                            
                            } else if (SynchBtn) {
                                SynchBtn = false;
                                ShowMessageBox ("Smart Sync", "Your App is already synchronized with the server..");
                            }
                            DataSynchSelecedValue = 0;

                        }
                    } else {
                        InvokeOnMainThread (() => {
                            
                            ShowMessageBox ("Smart Sync", "The WBid Synch server is not responding.  You can work on this bid and attempt to synch at a later time.");
                        });
                    }

                } else {
                    InvokeOnMainThread (() =>
                    {
                       
                        if (Reachability.IsSouthWestWifiOr2wire())
                        {
                            ShowMessageBox("Smart Sync", GlobalSettings.SouthWestConnectionAlert+"\nYou can work on this bid offline, but your state file for this bid may become unsynchronized from a previous state.");
                        }
                        else
                        {
                            ShowMessageBox("Smart Sync", "You do not have an internet connection.  You can work on this bid offline, but your state file for this bid may become unsynchronized from a previous state.");
                        }
                        NSApplication.SharedApplication.EndSheet (overlayPanel);
                        overlayPanel.OrderOut (this);
                    });
                }
            } catch (Exception ex) {
                throw ex;
            }
        }
  
        //setting overlay and text
        public void SetOverlay(string overlayText)
        {
            overlayPanel = new NSPanel();
            overlayPanel.SetContentSize(new CGSize(400, 120));
            overlay = new OverlayViewController();
            overlay.OverlayText = overlayText;
            overlayPanel.ContentView = overlay.View;
            NSApplication.SharedApplication.BeginSheet(overlayPanel, this.Window);
        }

       
        public WBStateInfoDTO GetWBServerStateandquicksetVersionNumber(WBGetStateDTO wbStateDTO)
        {
            WBStateInfoDTO versionInfo = null;
            try
            {
               // if (!GlobalSettings.SynchEnable)
                //    return versionInfo;
                string data = SmartSyncLogic.JsonObjectToStringSerializer<WBGetStateDTO>(wbStateDTO);
                string url = GlobalSettings.DataDownloadAuthenticationUrl + "GetWBServerStateandquicksetVersionNumber";
               
                string response = ServiceUtils.PostData(url, data);
                versionInfo = ConvertJSonToObject<WBStateInfoDTO>(response);
                return versionInfo;
            }
            catch (Exception ex)
            {
                versionInfo = null;
                IsSynchStart = false;
                return versionInfo;
            }

        }

        private bool GetWBStateAndquicksetFromServer(WBGetStateDTO wbStateDTO)
        {
            try
            {
                StateQuickSetSyncDTO stateQsSync = null;
                string url = GlobalSettings.DataDownloadAuthenticationUrl + "GetWBStateAndquicksetFromServer/";

                string data = SmartSyncLogic.JsonObjectToStringSerializer<WBGetStateDTO>(wbStateDTO);
                string response = ServiceUtils.PostData(url, data);
                response.Trim('"');

                stateQsSync = ConvertJSonToObject<StateQuickSetSyncDTO>(response);
                if (stateQsSync != null)
                {
                    if ((DataSynchSelecedValue != 3) || (DataSynchSelecedValue == 3 && IsStateFromServer && IsQSFromServer))
                    {
                        if (stateQsSync != null && stateQsSync.StateContent != null && stateQsSync.StateContent != string.Empty)
                        {

                            UpdateStateServerToLocal(stateQsSync);

                        }
                        UpdateQuickSetServerToLocal(stateQsSync);
                    }
                    else if (IsStateFromServer && IskeepLocalQS)
                        UpdateStateServerToLocal(stateQsSync);
                    else if (IsQSFromServer && IskeepLocalState)
                        UpdateQuickSetServerToLocal(stateQsSync);
                }
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        private void UpdateQuickSetServerToLocal(StateQuickSetSyncDTO stateQsSync)
        {
            try
            {
                if (stateQsSync != null && stateQsSync.QuickSetStateContent != null && stateQsSync.QuickSetStateContent != "")
                {
                    QuickSets quickset = null;
                    quickset = SmartSyncLogic.ConvertJsonToObject<QuickSets>(stateQsSync.QuickSetStateContent);
                    GlobalSettings.QuickSets = quickset;
                    GlobalSettings.QuickSets.QuickSetUpdatedTime = stateQsSync.QuickSetLastUpdatedTime;
                    GlobalSettings.QuickSets.SyncQuickSetVersion = stateQsSync.QuickSetVersionNumber.ToString();
                    GlobalSettings.QuickSets.IsModified = false;
                    //IsQSModified = false;
                    // IsStateModified = false;
                    XmlHelper.SerializeToXml(GlobalSettings.QuickSets, WBidHelper.GetQuickSetFilePath());
                }
            }
            catch (Exception ex)
            {
                throw (ex);
            }
        }
        private void UpdateStateServerToLocal(StateQuickSetSyncDTO stateQsSync)
        {
            bool failed = false;
            try
            {
                WBidStateCollection wBidStateCollection = null;
                bool isNeedToRecalculateLineProp = false;
                if (stateQsSync != null && stateQsSync.StateContent != null && stateQsSync.StateContent != string.Empty)
                {

                    wBidStateCollection = SmartSyncLogic.ConvertJsonToObject<WBidStateCollection>(stateQsSync.StateContent);
                    foreach (WBidState state in wBidStateCollection.StateList)
                    {
                        if (state.CxWtState.CLAuto == null)
                            state.CxWtState.CLAuto = new StateStatus { Cx = false, Wt = false };
                        if (state.CxWtState.StartDay == null)
                            state.CxWtState.StartDay = new StateStatus { Cx = false, Wt = false };
                        if (state.CxWtState.ReportRelease == null)
                            state.CxWtState.ReportRelease = new StateStatus { Cx = false, Wt = false };
                        if (state.CxWtState.CitiesLegs == null)
                        {
                            state.CxWtState.CitiesLegs = new StateStatus { Cx = false, Wt = false };
                            state.Constraints.CitiesLegs = new Cx3Parameters { ThirdcellValue = "1", Type = (int)ConstraintType.LessThan, Value = 1, lstParameters = new List<Cx3Parameter>() };
                            state.Weights.CitiesLegs = new Wt2Parameters
                            {
                                Type = 1,
                                Weight = 0,
                                lstParameters = new List<Wt2Parameter>()
                            };
                        }
                        if (state.CxWtState.Commute == null)
                        {
                            state.CxWtState.Commute = new StateStatus { Cx = false, Wt = false };
                            state.Constraints.Commute = new Commutability { BaseTime = 10, ConnectTime = 30, CheckInTime = 60, SecondcellValue = (int)CommutabilitySecondCell.NoMiddle, ThirdcellValue = (int)CommutabilityThirdCell.Overall, Type = (int)ConstraintType.MoreThan, Value = 100 };
                            state.Weights.Commute = new Commutability { BaseTime = 10, ConnectTime = 30, CheckInTime = 60, SecondcellValue = (int)CommutabilitySecondCell.NoMiddle, ThirdcellValue = (int)CommutabilityThirdCell.Overall, Type = (int)ConstraintType.MoreThan, Value = 100 };
                        }
                        state.CxWtState.Commute.Cx = false;
                        state.CxWtState.Commute.Wt = false;

                        if (state.Constraints.StartDayOftheWeek.SecondcellValue == null)
                        {
                            state.Constraints.StartDayOftheWeek.SecondcellValue = "1";
                            if (state.Constraints.StartDayOftheWeek.lstParameters != null)
                            {
                                foreach (var item in state.Constraints.StartDayOftheWeek.lstParameters)
                                {
                                    if (item.SecondcellValue == null)
                                    {
                                        item.SecondcellValue = "1";
                                    }
                                }
                            }
                        }

                        if (state.CxWtState.EQUIP.Cx)
                        {
                            state.Constraints.EQUIP.lstParameters.RemoveAll(x => x.ThirdcellValue == "500" || x.ThirdcellValue == "300");
                            if (state.Constraints.EQUIP.lstParameters.Count == 0)
                                state.CxWtState.EQUIP.Cx = false;
                        }
                        if (state.CxWtState.EQUIP.Wt)
                        {
                            state.Weights.EQUIP.lstParameters.RemoveAll(x => x.SecondlValue == 500 || x.SecondlValue == 300);
                            if (state.Weights.EQUIP.lstParameters.Count == 0)
                                state.CxWtState.EQUIP.Wt = false;
                        }



                    }
                    foreach (var item in wBidStateCollection.StateList)
                    {
                        if (item.BidAuto != null && item.BidAuto.BAFilter != null && item.BidAuto.BAFilter.Count > 0)
                        {
                            HandleTypeOfBidAutoObject(item.BidAuto.BAFilter);

                        }
                        if (item.CalculatedBA != null && item.CalculatedBA.BAFilter != null && item.CalculatedBA.BAFilter.Count > 0)
                        {
                            HandleTypeOfBidAutoObject(item.CalculatedBA.BAFilter);

                        }

                    }


                    var wBidStateContent = wBidStateCollection.StateList.FirstOrDefault(x => x.StateName == GlobalSettings.WBidStateCollection.DefaultName);


                    var currentopendState = GlobalSettings.WBidStateCollection.StateList.FirstOrDefault(x => x.StateName == GlobalSettings.WBidStateCollection.DefaultName);
                    try
                    {

                        if (wBidStateContent.BidAuto != null && wBidStateContent.BidAuto.BAFilter != null && wBidStateContent.BidAuto.BAFilter.Count > 0)
                        {
                            wBidStateContent.BidAuto.BAFilter.RemoveAll(x => x.Name == "ET" && ((Cx3Parameter)x.BidAutoObject).ThirdcellValue == "500");
                            wBidStateContent.BidAuto.BAFilter.RemoveAll(x => x.Name == "ET" && ((Cx3Parameter)x.BidAutoObject).ThirdcellValue == "300");
                        }
                        if (wBidStateContent.CalculatedBA != null && wBidStateContent.CalculatedBA.BAFilter != null && wBidStateContent.CalculatedBA.BAFilter.Count > 0)
                        {
                            wBidStateContent.CalculatedBA.BAFilter.RemoveAll(x => x.Name == "ET" && ((Cx3Parameter)x.BidAutoObject).ThirdcellValue == "500");
                            wBidStateContent.CalculatedBA.BAFilter.RemoveAll(x => x.Name == "ET" && ((Cx3Parameter)x.BidAutoObject).ThirdcellValue == "300");
                        }

                    }
                    catch (Exception ex)
                    {

                    }
                    if (wBidStateContent.MenuBarButtonState.IsOverlap && currentopendState.IsOverlapCorrection == false)
                    {
                        InvokeOnMainThread(() =>
                        {
                            ShowMessageBox("Smart Sync", "Previous state had Overlap Data and You need to re-download the bid data and make an overlap correction");
                            //syncOverlay.Hide();
                           // UIAlertController okAlertController = UIAlertController.Create("Smart Sync", "Previous state had Overlap Data and You need to re-download the bid data and make an overlap correction", UIAlertControllerStyle.Alert);
                           // okAlertController.AddAction(UIAlertAction.Create("OK", UIAlertActionStyle.Default, null));
                           // this.PresentViewController(okAlertController, true, null);
                        });

                        return;
                    }


                    StateManagement statemanagement = new StateManagement();

                    isNeedToRecalculateLineProp = statemanagement.CheckLinePropertiesNeedToRecalculate(wBidStateContent);
                    ResetLinePropertiesBackToNormal(currentopendState, wBidStateContent);
                    ResetOverlapState(currentopendState, wBidStateContent);

                    GlobalSettings.WBidStateCollection = wBidStateCollection;
                    //GlobalSettings.WBidStateContent = GlobalSettings.WBidStateCollection.StateList.FirstOrDefault(x => x.StateName == GlobalSettings.WBidStateCollection.DefaultName); ;
                    GlobalSettings.WBidStateCollection.SyncVersion = stateQsSync.VersionNumber.ToString();
                    GlobalSettings.WBidStateCollection.StateUpdatedTime = stateQsSync.LastUpdatedTime;
                    GlobalSettings.WBidStateCollection.IsModified = false;

                    string currentBidName = WBidHelper.GenerateFileNameUsingCurrentBidDetails();

                    //string zipFileName = GenarateZipFileName();
                    string vACFileName = WBidHelper.GetAppDataPath() + "//" + currentBidName + ".VAC";
                    //Cheks the VAC file exists
                    bool vacFileExists = File.Exists(vACFileName);
                    if (vacFileExists == false)
                    {
                        wBidStateContent.MenuBarButtonState.IsVacationDrop = false;
                        wBidStateContent.MenuBarButtonState.IsVacationCorrection = false;
                        wBidStateContent.IsVacationOverlapOverlapCorrection = false;
                    }
                    if (wBidStateContent.MenuBarButtonState.IsEOM)
                    {
                        SetEOMVacationDataAfterSynch();
                    }
                    if (wBidStateContent.MenuBarButtonState.IsMIL)
                    {
                        SetMILDataAfterSynch();
                    }
                    if (wBidStateContent.MenuBarButtonState.IsMIL && isNeedtoCreateMILFile)
                    {
                        isNeedToRecalculateLineProp = true;
                    }
                    wBidStateContent.SortDetails.SortColumn = "Manual";

                    bool isCommuteAutoAvailable = IsCommuteAutoAvailable();
                    if (isCommuteAutoAvailable == true && File.Exists(WBidHelper.WBidCommuteFilePath))
                    {
                        File.Delete(WBidHelper.WBidCommuteFilePath);
                    }
                    string stateFilePath = Path.Combine(WBidHelper.GetAppDataPath(), stateQsSync.StateFileName + ".WBS");
                    //WBidCollection.SaveStateFile(GlobalSettings.WBidStateCollection, stateFilePath);
                    WBidHelper.SaveStateFile(WBidHelper.WBidStateFilePath);



                }
                InvokeOnMainThread(() =>
                {
                   // UIAlertController okAlertController = UIAlertController.Create("Smart Sync", "Successfully Synchronized  your computer with the server.", UIAlertControllerStyle.Alert);
                   // okAlertController.AddAction(UIAlertAction.Create("OK", UIAlertActionStyle.Default, null));
                   //// this.PresentViewController(okAlertController, true, null);
                //    syncOverlay.Hide();
                   if(CommonClass.BAController != null)CommonClass.BAController.ReloadAllContent();

                    if (isNeedToClose == 0) {
                        GlobalSettings.Lines.ToList ().ForEach (x => {
                            x.ConstraintPoints.Reset ();
                            x.Constrained = false;
                            x.WeightPoints.Reset ();
                            x.TotWeight = 0.0m;
                        });
                        StateManagement statemanagement = new StateManagement ();
                        statemanagement.ReloadLineDetailsBasedOnSynchedState (isNeedtoCreateMILFile);
                        //statemanagement.ReloadDataFromStateFile ();
                        CommonClass.ViewChanged = true;
                        var wBidStateContent = wBidStateCollection.StateList.FirstOrDefault (x => x.StateName == GlobalSettings.WBidStateCollection.DefaultName);
                        if (wBidStateContent.TagDetails != null) {
                            foreach (var item in GlobalSettings.Lines) {
                                var tagItem = wBidStateContent.TagDetails.FirstOrDefault (x => x.Line == item.LineNum);
                                if (tagItem != null)
                                    item.Tag = tagItem.Content;
                                else
                                    item.Tag = string.Empty;

                            }

                        }
                        ReloadAllContent ();
                        CommonClass.ViewChanged = false;
                        if (CommonClass.CSWController != null) {
                            CommonClass.CSWController.ReloadAllContent ();
                        }
                        SetVacButtonStates ();
                        NSApplication.SharedApplication.EndSheet (overlayPanel);
                        overlayPanel.OrderOut (this);
                        //ShowMessageBox ("Smart Sync", "Successfully Synchronized  your computer with the server.");

                    }
                   
                    else
                    {

                        var alert = new NSAlert();
                        alert.Window.Title = "Smart Sync";
                        alert.MessageText = "Successfully Synchronized  your computer with the server.";
                        //alert.InformativeText = "There are no Latest News available..!";
                        alert.AddButton("OK");
                        //alert.AddButton ("Cancel");
                        alert.Buttons[0].Activated += delegate {
                            alert.Window.Close();
                            NSApplication.SharedApplication.StopModal();
                            if (isNeedToClose == 1)
                                GoToHomeWindow();
                            else if (isNeedToClose == 2)
                                ExitApp();
                        };
                        alert.RunModal();
                    }
                    FirstTime = false;
                });
            }
            catch (Exception ex)
            {
                FirstTime = false;
                failed = true;

            }

            if (failed)
            {
                InvokeOnMainThread(() =>
                {
                    NSApplication.SharedApplication.EndSheet(overlayPanel);
                    overlayPanel.OrderOut(this);
                    ShowMessageBox("Smart Sync", "The server to get the previous state file is not responding.  You can work on this bid, but your state file for this bid may become unsynchronized from a previous state.");

                   // syncOverlay.Hide();
                    //UIAlertController okAlertController = UIAlertController.Create("Smart Sync", "The server to get the previous state file is not responding.  You can work on this bid, but your state file for this bid may become unsynchronized from a previous state.", UIAlertControllerStyle.Alert);
                    //okAlertController.AddAction(UIAlertAction.Create("OK", UIAlertActionStyle.Default, null));
                    //this.PresentViewController(okAlertController, true, null);

                });

            }

        }
        public bool SaveWBStateAndQuickSetToServer(string stateFileName)
        {
            try
            {
                string url = GlobalSettings.DataDownloadAuthenticationUrl + "SaveWBStateAndQuickSetToServer/";
                WBidStateCollection wBidStateCollection = GlobalSettings.WBidStateCollection;

                foreach (var item in wBidStateCollection.StateList)
                {
                    if (item.FAEOMStartDate == DateTime.MinValue)
                    {
                        item.FAEOMStartDate = DateTime.MinValue.ToUniversalTime();
                    }

                }


                StateQuickSetSyncDTO stateSync = new StateQuickSetSyncDTO();
                stateSync.EmployeeNumber = GlobalSettings.WbidUserContent.UserInformation.EmpNo;
                stateSync.StateFileName = stateFileName;
                stateSync.VersionNumber = int.Parse(GlobalSettings.WBidStateCollection.SyncVersion);
                stateSync.Year = GlobalSettings.CurrentBidDetails.Year;
                stateSync.StateContent = "";
                stateSync.LastUpdatedTime = DateTime.MinValue.ToUniversalTime();
                stateSync.QuickSetFileName = GlobalSettings.WbidUserContent.UserInformation.EmpNo;
                stateSync.QuickSetVersionNumber = 0;
                if (GlobalSettings.QuickSets != null)
                {
                    if (GlobalSettings.QuickSets.SyncQuickSetVersion != null)
                        stateSync.QuickSetVersionNumber = int.Parse(GlobalSettings.QuickSets.SyncQuickSetVersion);
                }
                stateSync.QuickSetLastUpdatedTime = DateTime.MinValue.ToUniversalTime();
                stateSync.QuickSetStateContent = "";
                if (DataSynchSelecedValue == 1 || DataSynchSelecedValue == 3)
                {
                    string stateContent = SmartSyncLogic.JsonObjectToStringSerializer<WBidStateCollection>(wBidStateCollection);
                    stateSync.StateContent = stateContent != null ? stateContent : null;
                }
                if (DataSynchSelecedValue == 2 || DataSynchSelecedValue == 3)
                {
                    QuickSets quickset = GlobalSettings.QuickSets;
                    string quickSetContent = SmartSyncLogic.JsonObjectToStringSerializer<QuickSets>(quickset);
                    stateSync.QuickSetStateContent = quickSetContent != null ? quickSetContent : null;
                }
                string data = SmartSyncLogic.JsonObjectToStringSerializer<StateQuickSetSyncDTO>(stateSync);

                string response = ServiceUtils.PostData(url, data);
                response.Trim('"');
                WBStateInfoDTO wbStateInfoDTO = ConvertJSonToObject<WBStateInfoDTO>(response);

                if (DataSynchSelecedValue == 1 || DataSynchSelecedValue == 3)
                {
                    GlobalSettings.WBidStateCollection.SyncVersion = wbStateInfoDTO.StateVersionNumber;
                    GlobalSettings.WBidStateCollection.StateUpdatedTime = wbStateInfoDTO.StateLastUpdate;

                    GlobalSettings.WBidStateCollection.IsModified = false;
                    string stateFilePath = Path.Combine(WBidHelper.GetAppDataPath(), stateFileName + ".WBS");
                    WBidHelper.SaveStateFile(WBidHelper.WBidStateFilePath);

                    IsSynchStart = false;
                }
                if (DataSynchSelecedValue == 2 || DataSynchSelecedValue == 3)
                {
                    GlobalSettings.QuickSets.SyncQuickSetVersion = wbStateInfoDTO.QuickSetVersionNumber;
                    GlobalSettings.QuickSets.QuickSetUpdatedTime = wbStateInfoDTO.QuickSetLastUpdated;
                    GlobalSettings.QuickSets.IsModified= false;
                    IsSynchStart = false; XmlHelper.SerializeToXml(GlobalSettings.QuickSets, WBidHelper.GetQuickSetFilePath());
                }
                return true;
            }
            catch (Exception ex)
            {
                IsSynchStart = false;
                return false;
            }
        }

        private void GetStateFromServer (string stateFileName)
        {
            bool failed = false;
            try {
                string url = GlobalSettings.synchServiceUrl + "GetWBidStateFromServer/" + GlobalSettings.WbidUserContent.UserInformation.EmpNo + "/" + stateFileName + "/" + GlobalSettings.CurrentBidDetails.Year;


                HttpWebRequest request = (HttpWebRequest)WebRequest.Create (url);
                request.Timeout = 30000;
                HttpWebResponse response = (HttpWebResponse)request.GetResponse ();
                var stream = response.GetResponseStream ();
                var reader = new StreamReader (stream);
                StateSync stateSync = SmartSyncLogic.ConvertJsonToObject<StateSync> (reader.ReadToEnd ());
                WBidStateCollection wBidStateCollection = null;
                bool isNeedToRecalculateLineProp = false;
                if (stateSync != null) {

                    // clear the BA filter item if the BA view open
                
                    wBidStateCollection = SmartSyncLogic.ConvertJsonToObject<WBidStateCollection> (stateSync.StateContent);
                    foreach (WBidState state in wBidStateCollection.StateList)
                    {
                        if (state.CxWtState.CLAuto == null)
                            state.CxWtState.CLAuto = new StateStatus { Cx = false, Wt = false };
                        if (state.CxWtState.CitiesLegs == null)
                        {
                            state.CxWtState.CitiesLegs = new StateStatus() { Cx = false, Wt = false };
                            state.Constraints.CitiesLegs=new Cx3Parameters { ThirdcellValue = "1", Type = (int)ConstraintType.LessThan, Value = 1 ,lstParameters=new List<Cx3Parameter>()};
                            state.Weights.CitiesLegs = new Wt2Parameters {
                                Type = 1,
                                Weight = 0,
                                lstParameters = new List<Wt2Parameter> ()
                            };

                        }
                        //state.SortDetails.BlokSort.RemoveAll(x=>x=="30"||x=="31"||x=="32");
                        if(state.CxWtState.Commute==null)
                            state.CxWtState.Commute = new StateStatus { Wt = false, Cx = false };
                        if (state.Constraints.Commute == null)
                        {
                            state.Constraints.Commute = new Commutability { BaseTime = 10, ConnectTime = 30, CheckInTime = 60, SecondcellValue = (int)CommutabilitySecondCell.NoMiddle, ThirdcellValue = (int)CommutabilityThirdCell.Overall, Type = (int)ConstraintType.MoreThan, Value = 100 };
                        }
                        if (state.Weights.Commute == null)
                        {
                            state.Weights.Commute = new Commutability { BaseTime = 10, ConnectTime = 30, CheckInTime = 60, SecondcellValue = (int)CommutabilitySecondCell.NoMiddle, ThirdcellValue = (int)CommutabilityThirdCell.Overall, Type = (int)ConstraintType.MoreThan, Value = 100,Weight=0 };
                        }
                        if (state.Constraints.StartDayOftheWeek.SecondcellValue == null)
                        {
                            state.Constraints.StartDayOftheWeek.SecondcellValue = "1";
                            if (state.Constraints.StartDayOftheWeek.lstParameters != null)
                            {
                                foreach (var item in state.Constraints.StartDayOftheWeek.lstParameters)
                                {
                                    if (item.SecondcellValue == null)
                                    {
                                        item.SecondcellValue = "1";
                                    }
                                }
                            }
                        }
                    }
                    foreach (var item in wBidStateCollection.StateList)
                    {
                        if (item.BidAuto != null && item.BidAuto.BAFilter != null && item.BidAuto.BAFilter.Count > 0)
                        {
                            HandleTypeOfBidAutoObject(item.BidAuto.BAFilter);
                        }
                        if (item.CalculatedBA != null && item.CalculatedBA.BAFilter != null && item.CalculatedBA.BAFilter.Count > 0)
                        {
                            HandleTypeOfBidAutoObject(item.CalculatedBA.BAFilter);
                        }
                    }
                    var wBidStateContent = wBidStateCollection.StateList.FirstOrDefault (x => x.StateName == GlobalSettings.WBidStateCollection.DefaultName);
                    var currentopendState = GlobalSettings.WBidStateCollection.StateList.FirstOrDefault (x => x.StateName == GlobalSettings.WBidStateCollection.DefaultName);

                    foreach (WBidState state in wBidStateCollection.StateList)
                    {
                        state.Constraints.EQUIP.ThirdcellValue = "700";
                        state.Constraints.EQUIP.lstParameters.RemoveAll(x => x.ThirdcellValue == "300" || x.ThirdcellValue == "500");
                        if (state.Constraints.EQUIP.lstParameters.Count < 1)

                            state.CxWtState.EQUIP.Cx = false;
                        state.Weights.EQUIP.SecondlValue = 700;
                        state.Weights.EQUIP.lstParameters.RemoveAll(x => x.SecondlValue == 300 || x.SecondlValue == 500);
                        if (state.Weights.EQUIP.lstParameters.Count < 1)

                            state.CxWtState.EQUIP.Wt = false;

                        //remove 300 and 500 equipments
                        if (state.BidAuto != null && state.BidAuto.BAFilter != null && state.BidAuto.BAFilter.Count > 0)
                        {
                            state.BidAuto.BAFilter.RemoveAll(x => x.Name == "ET" && ((Cx3Parameter)x.BidAutoObject).ThirdcellValue == "500");
                            state.BidAuto.BAFilter.RemoveAll(x => x.Name == "ET" && ((Cx3Parameter)x.BidAutoObject).ThirdcellValue == "300");
                            int count = 0;
                            foreach (var item in state.BidAuto.BAFilter)
                            {
                                item.Priority = count;
                                count++;
                            }
                        }
                        if (state.CalculatedBA != null && state.CalculatedBA.BAFilter != null && state.CalculatedBA.BAFilter.Count > 0)
                        {
                            state.CalculatedBA.BAFilter.RemoveAll(x => x.Name == "ET" && ((Cx3Parameter)x.BidAutoObject).ThirdcellValue == "500");
                            state.CalculatedBA.BAFilter.RemoveAll(x => x.Name == "ET" && ((Cx3Parameter)x.BidAutoObject).ThirdcellValue == "300");
                        }
                    }
                    StateManagement statemanagement = new StateManagement ();

                    isNeedToRecalculateLineProp = statemanagement.CheckLinePropertiesNeedToRecalculate (wBidStateContent);
                    ResetLinePropertiesBackToNormal (currentopendState, wBidStateContent);
                    ResetOverlapState (currentopendState, wBidStateContent);
                    GlobalSettings.WBidStateCollection = wBidStateCollection;
                    //GlobalSettings.WBidStateContent = GlobalSettings.WBidStateCollection.StateList.FirstOrDefault(x => x.StateName == GlobalSettings.WBidStateCollection.DefaultName); ;
                    GlobalSettings.WBidStateCollection.SyncVersion = stateSync.VersionNumber.ToString ();
                    GlobalSettings.WBidStateCollection.StateUpdatedTime = stateSync.LastUpdatedTime;
                    GlobalSettings.WBidStateCollection.IsModified = false;

                    if (wBidStateContent.MenuBarButtonState.IsEOM) {
                        SetEOMVacationDataAfterSynch ();
                    }
                    if (wBidStateContent.MILDateList!=null && wBidStateContent.MILDateList.Count > 0) {
                        isNeedtoCreateMILFile = false;
                        SetMILDataAfterSynch ();
                    }
                    WBidHelper.SaveStateFile (WBidHelper.WBidStateFilePath);


                }
                InvokeOnMainThread (() => {


                    if(CommonClass.BAController != null)CommonClass.BAController.ReloadAllContent();

                    if (isNeedToClose == 0) {
                        GlobalSettings.Lines.ToList ().ForEach (x => {
                            x.ConstraintPoints.Reset ();
                            x.Constrained = false;
                            x.WeightPoints.Reset ();
                            x.TotWeight = 0.0m;
                        });
                        StateManagement statemanagement = new StateManagement ();
                        statemanagement.ReloadLineDetailsBasedOnSynchedState (isNeedtoCreateMILFile);
                        //statemanagement.ReloadDataFromStateFile ();
                        CommonClass.ViewChanged = true;
                        var wBidStateContent = wBidStateCollection.StateList.FirstOrDefault (x => x.StateName == GlobalSettings.WBidStateCollection.DefaultName);
                        if (wBidStateContent.TagDetails != null) {
                            foreach (var item in GlobalSettings.Lines) {
                                var tagItem = wBidStateContent.TagDetails.FirstOrDefault (x => x.Line == item.LineNum);
                                if (tagItem != null)
                                    item.Tag = tagItem.Content;
                                else
                                    item.Tag = string.Empty;

                            }

                        }
                        ReloadAllContent ();
                        CommonClass.ViewChanged = false;
                        if (CommonClass.CSWController != null) {
                            CommonClass.CSWController.ReloadAllContent ();
                        }
                        SetVacButtonStates ();
                        NSApplication.SharedApplication.EndSheet (overlayPanel);
                        overlayPanel.OrderOut (this);
                        ShowMessageBox ("Smart Sync", "Successfully Synchronized  your computer with the server.");

                    } else {

                        var alert = new NSAlert ();
                        alert.Window.Title = "Smart Sync";
                        alert.MessageText = "Successfully Synchronized  your computer with the server.";
                        //alert.InformativeText = "There are no Latest News available..!";
                        alert.AddButton ("OK");
                        //alert.AddButton ("Cancel");
                        alert.Buttons [0].Activated += delegate {
                            alert.Window.Close ();
                            NSApplication.SharedApplication.StopModal ();
                            if (isNeedToClose == 1)
                                GoToHomeWindow ();
                            else if (isNeedToClose == 2)
                                ExitApp ();
                        };
                        alert.RunModal ();
                    }
                    FirstTime = false;
                });
            } catch (Exception ex) {


                FirstTime = false;
                failed = true;

            }

            if (failed) {
                InvokeOnMainThread (() => {

                    NSApplication.SharedApplication.EndSheet (overlayPanel);
                    overlayPanel.OrderOut (this);
                    ShowMessageBox ("Smart Sync", "The server to get the previous state file is not responding.  You can work on this bid, but your state file for this bid may become unsynchronized from a previous state.");

                });

            }

        }

        private void HandleTypeOfBidAutoObject(List<BidAutoItem> filterList)
        {
            foreach (var filter in filterList)
            {
                if (filter.BidAutoObject.GetType().Name == "JObject")
                {
                    switch (filter.Name)
                    {
                    case "AP":
                        filter.BidAutoObject = ((JObject)filter.BidAutoObject).ToObject<AMPMConstriants>();
                        break;
                    case "DOWA":
                        filter.BidAutoObject = ((JObject)filter.BidAutoObject).ToObject<CxDays>();
                        break;
                    case "DOWS":
                        filter.BidAutoObject = ((JObject)filter.BidAutoObject).ToObject<Cx3Parameter>();

                        break;
                    case "DHFL":
                        filter.BidAutoObject = ((JObject)filter.BidAutoObject).ToObject<Cx3Parameter>();
                        break;
                    case "ET":
                        filter.BidAutoObject = ((JObject)filter.BidAutoObject).ToObject<Cx3Parameter>();
                        break;
                    case "RT":
                        filter.BidAutoObject = ((JObject)filter.BidAutoObject).ToObject<Cx3Parameter>();
                        break;
                    case "LT":
                        filter.BidAutoObject = ((JObject)filter.BidAutoObject).ToObject<CxLine>();

                        break;
                    case "TBL":
                        filter.BidAutoObject = ((JObject)filter.BidAutoObject).ToObject<CxTripBlockLength>();
                        break;
                    case "SDOW":
                        filter.BidAutoObject = ((JObject)filter.BidAutoObject).ToObject<CxDays>();
                        break;
                    case "DOM":
                        filter.BidAutoObject = ((JObject)filter.BidAutoObject).ToObject<DaysOfMonthCx>();
                        break;
                    case "CL":
                        filter.BidAutoObject = ((JObject)filter.BidAutoObject).ToObject<FtCommutableLine>();
                        break;
                    case "OC":
                        filter.BidAutoObject = ((JObject)filter.BidAutoObject).ToObject<BulkOvernightCityCx>();
                        break;
                    }

                }
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="currentState"></param>
        /// <param name="newState"></param>
        private void ResetLinePropertiesBackToNormal (WBidState currentState, WBidState newState)
        {
            if (newState.MenuBarButtonState.IsOverlap == false && currentState.MenuBarButtonState.IsOverlap) {
                //remove the  Overlp Calculation from line
                ReCalculateLinePropertiesForOverlapCorrection reCalculateLinePropertiesForOverlapCorrection = new ReCalculateLinePropertiesForOverlapCorrection ();
                reCalculateLinePropertiesForOverlapCorrection.ReCalculateLinePropertiesOnOverlapCorrection (GlobalSettings.Lines.ToList (), false);
            } else if ((currentState.MenuBarButtonState.IsVacationCorrection || currentState.MenuBarButtonState.IsEOM) && newState.MenuBarButtonState.IsOverlap) {
                GlobalSettings.MenuBarButtonStatus.IsEOM = false;
                GlobalSettings.MenuBarButtonStatus.IsVacationCorrection = false;
                GlobalSettings.MenuBarButtonStatus.IsVacationDrop = false;
                //Remove the vacation propertiesfrom Line 
                RecalcalculateLineProperties RecalcalculateLineProperties = new RecalcalculateLineProperties ();
                RecalcalculateLineProperties.CalcalculateLineProperties ();
            }

        }


        private void ResetOverlapState (WBidState currentState, WBidState newState)
        {
            if (newState.IsOverlapCorrection == false && currentState.IsOverlapCorrection) {
                newState.IsOverlapCorrection = true;
            } else if (newState.IsOverlapCorrection && currentState.IsOverlapCorrection == false) {
                newState.IsOverlapCorrection = false;
            }
        }

        private void ConvertVacationDateFormat ()
        {
            if (GlobalSettings.WBidStateCollection.Vacation.Count () > 0) {
                for (int count = 0; count < GlobalSettings.WBidStateCollection.Vacation.Count (); count++) {
                    //  GlobalSettings.WBidStateCollection.Vacation[count].StartDate = DateTime.Parse(GlobalSettings.WBidStateCollection.Vacation[count].StartDate).ToShortDateString();
                    // GlobalSettings.WBidStateCollection.Vacation[count].EndDate = DateTime.Parse(GlobalSettings.WBidStateCollection.Vacation[count].EndDate).ToShortDateString();
                    if (GlobalSettings.WBidStateCollection.Vacation [count].StartDate.Contains ("/")) {
                        string[] split = GlobalSettings.WBidStateCollection.Vacation [count].StartDate.Split ('/');
                        GlobalSettings.WBidStateCollection.Vacation [count].StartDate = new DateTime (int.Parse (split [2]), int.Parse (split [0]), int.Parse (split [1])).ToShortDateString ();
                        split = GlobalSettings.WBidStateCollection.Vacation [count].EndDate.Split ('/');
                        GlobalSettings.WBidStateCollection.Vacation [count].EndDate = new DateTime (int.Parse (split [2]), int.Parse (split [0]), int.Parse (split [1])).ToShortDateString ();

                    }
                }
            }
        }

        public static T ConvertJSonToObject<T> (string jsonString)
        {
            DataContractJsonSerializer serializer = new DataContractJsonSerializer (typeof(T));
            MemoryStream ms = new MemoryStream (Encoding.UTF8.GetBytes (jsonString));
            T obj = (T)serializer.ReadObject (ms);
            return obj;
        }

        private void SynchState ()
        {
            try {
                bool isConnectionAvailable = Reachability.CheckVPSAvailable ();
                if (isConnectionAvailable) {
                    //new thread?
                    IsSynchStart = true;

                    string stateFileName = WBidHelper.GenerateFileNameUsingCurrentBidDetails ();
                    SynchStateVersion = int.Parse (GlobalSettings.WBidStateCollection.SyncVersion);
                    //Get server State Version
                    VersionInfo versionInfo = GetServerVersion (stateFileName);
                    if (versionInfo != null) {
                        ServerSynchTime = DateTime.Parse (versionInfo.LastUpdatedDate, CultureInfo.InvariantCulture);

                        if (versionInfo.Version != string.Empty) {
                            int serverVersion = Convert.ToInt32 (versionInfo.Version);

                            if (SynchStateVersion != serverVersion || GlobalSettings.WBidStateCollection.IsModified) {
                                //conflict
                                confNotif = NSNotificationCenter.DefaultCenter.AddObserver ((NSString)"SyncConflict", (NSNotification notification) => {
                                    string str = notification.Object.ToString ();
                                    NSNotificationCenter.DefaultCenter.RemoveObserver (confNotif);
                                    BeginInvokeOnMainThread (() => {
                                        if (str == "server") {
                                            overlayPanel = new NSPanel ();
                                            overlayPanel.SetContentSize (new CGSize (400, 120));
                                            overlay = new OverlayViewController ();
                                            overlay.OverlayText = "Synching current State FROM server \n Please wait..";
                                            overlayPanel.ContentView = overlay.View;
                                            NSApplication.SharedApplication.BeginSheet (overlayPanel, this.Window);

                                            BeginInvokeOnMainThread (() => {
                                                GetStateFromServer (stateFileName);
                                            }
                                            );
                                        } else if (str == "local") {
                                            overlayPanel = new NSPanel ();
                                            overlayPanel.SetContentSize (new CGSize (400, 120));
                                            overlay = new OverlayViewController ();
                                            overlay.OverlayText = "Synching current State TO server \n Please wait..";
                                            overlayPanel.ContentView = overlay.View;
                                            NSApplication.SharedApplication.BeginSheet (overlayPanel, this.Window);

                                            BeginInvokeOnMainThread (() => {
                                                UploadLocalVersionToServer (stateFileName);
                                            });
                                        } else {
                                            IsSynchStart = false;
                                            GoToHomeWindow (); //GoToHome ();
                                        }
                                        //GoToHome ();
                                    });
                                });
                            
                                var panel = new NSPanel ();
                                var synchConf = new SynchConflictViewController ();
                                synchConf.serverSynchTime = ServerSynchTime;
                                if (serverVersion == 0)
                                    synchConf.noServer = true;
                                CommonClass.Panel = panel;
                                panel.SetContentSize (new CGSize (450, 285));
                                panel.ContentView = synchConf.View;
                                NSApplication.SharedApplication.BeginSheet (panel, CommonClass.MainController.Window);
                                //NSApplication.SharedApplication.RunModalForWindow (panel);
                            
                            }


                        }
                    } else {
                        InvokeOnMainThread (() => {
                            ShowMessageBox ("Smart Sync", "The WBid Synch server is not responding.  You can work on this bid and attempt to synch at a later time.");
                        });
                    }
                } else {
                    InvokeOnMainThread (() => {
                        if (Reachability.IsSouthWestWifiOr2wire())
                        {
                            ShowMessageBox("Smart Sync", GlobalSettings.SouthWestConnectionAlert + "\nYou can work on this bid offline, but your state file for this bid may become unsynchronized from a previous state.");
                        }
                        else
                        {
                            ShowMessageBox("Smart Sync", "You do not have an internet connection.  You can work on this bid offline, but your state file for this bid may become unsynchronized from a previous state.");
                        }
                        //ShowMessageBox ("Smart Sync", "You do not have an internet connection.  You can work on this bid offline, but your state file for this bid may become unsynchronized from a previous state.");
                    });
                }
            } catch (Exception ex) {
                throw ex;
            }

        }

        private void UploadLocalVersionToServer (string stateFileName)
        {
            int version = int.Parse (SaveStateToServer (stateFileName));
            if (version != -1) {
                GlobalSettings.WBidStateCollection.SyncVersion = version.ToString ();
                GlobalSettings.WBidStateCollection.StateUpdatedTime = DateTime.Now.ToUniversalTime ();
                GlobalSettings.WBidStateCollection.IsModified = false;
                string stateFilePath = Path.Combine (WBidHelper.GetAppDataPath (), stateFileName + ".WBS");
                //WBidCollection.SaveStateFile(GlobalSettings.WBidStateCollection, stateFilePath);
                WBidHelper.SaveStateFile (WBidHelper.WBidStateFilePath);

                IsSynchStart = false;
                InvokeOnMainThread (() => {
                    NSApplication.SharedApplication.EndSheet (overlayPanel);
                    overlayPanel.OrderOut (this);

                    if (isNeedToClose == 0)
                        ShowMessageBox ("Smart Sync", "Successfully Synchronized  your computer with the server.");
                    else {
                        var alert = new NSAlert ();
                        alert.Window.Title = "Smart Sync";
                        alert.MessageText = "Successfully Synchronized  your computer with the server.";
                        //alert.InformativeText = "There are no Latest News available..!";
                        alert.AddButton ("OK");
                        //alert.AddButton ("Cancel");
                        alert.Buttons [0].Activated += delegate {
                            alert.Window.Close ();
                            NSApplication.SharedApplication.StopModal ();
                            if (isNeedToClose == 1)
                                GoToHomeWindow ();
                            else if (isNeedToClose == 2)
                                ExitApp ();
                        };
                        alert.RunModal ();

                    }

                    FirstTime = false;
                });
            } else {
                InvokeOnMainThread (() => {
                    NSApplication.SharedApplication.EndSheet (overlayPanel);
                    overlayPanel.OrderOut (this);
                    ShowMessageBox ("Smart Sync", "An error occured while synchronizing your state to the server.  You can work on this bid, but your state file for this bid may become unsynchronized from a previous state.");
                });
            }
        }

        private VersionInfo GetServerVersion (string stateFileName)
        {
            VersionInfo versionInfo = null;
            try {
                if (!GlobalSettings.WBidINIContent.User.SmartSynch)
                    return versionInfo;
                string url = GlobalSettings.synchServiceUrl + "GetServerStateVersionNumber/" + GlobalSettings.WbidUserContent.UserInformation.EmpNo + "/" + stateFileName + "/" + GlobalSettings.CurrentBidDetails.Year;
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create (url);
                request.Timeout = 30000;
                HttpWebResponse response = (HttpWebResponse)request.GetResponse ();
                var stream = response.GetResponseStream ();
                var reader = new StreamReader (stream);
                versionInfo = ConvertJSonToObject<VersionInfo> (reader.ReadToEnd ());
                versionInfo.Version = versionInfo.Version.Trim ('"');
                return versionInfo;
            } catch (Exception ex) {
                versionInfo = null;
                IsSynchStart = false;
                return versionInfo;
                //throw ex;
            }
        }

        private string SaveStateToServer (string stateFileName)
        {
            try {
                string url = GlobalSettings.synchServiceUrl + "SaveWBidStateToServer/";
                WBidStateCollection wBidStateCollection = GlobalSettings.WBidStateCollection;

                foreach (var item in wBidStateCollection.StateList) {
                    if (item.FAEOMStartDate == DateTime.MinValue) {
                        item.FAEOMStartDate = DateTime.MinValue.ToUniversalTime ();
                    }

                }

                string data = string.Empty;
                StateSync stateSync = new StateSync ();
                stateSync.EmployeeNumber = GlobalSettings.WbidUserContent.UserInformation.EmpNo;
                stateSync.StateFileName = stateFileName;
                stateSync.VersionNumber = 0;
                stateSync.Year = GlobalSettings.CurrentBidDetails.Year;
                stateSync.StateContent = SmartSyncLogic.JsonObjectToStringSerializer<WBidStateCollection> (wBidStateCollection);
                stateSync.LastUpdatedTime = DateTime.MinValue.ToUniversalTime ();

                var request = (HttpWebRequest)WebRequest.Create (url);
                request.Method = "POST";
                request.ContentType = "application/x-www-form-urlencoded";
                //data = SmartSyncLogic.JsonSerializer(stateSync);

                data = SmartSyncLogic.JsonObjectToStringSerializer<StateSync> (stateSync);
                var bytes = Encoding.UTF8.GetBytes (data);
                request.ContentLength = bytes.Length;
                request.GetRequestStream ().Write (bytes, 0, bytes.Length);
                request.Timeout = 30000;
                //Response
                var response = (HttpWebResponse)request.GetResponse ();
                var stream = response.GetResponseStream ();
                if (stream == null)
                    return string.Empty;

                var reader = new StreamReader (stream);
                string result = reader.ReadToEnd ();

                return result.Trim ('"');
            } catch (Exception ex) {
                IsSynchStart = false;
                return "-1";
            }
        }


        static void SetPropertyNames ()
        {
            if (GlobalSettings.MenuBarButtonStatus == null)
                GlobalSettings.MenuBarButtonStatus = new MenuBarButtonStatus ();

            CommonClass.bidLineProperties = new List<string> ();
            CommonClass.modernProperties = new List<string> ();

            if (GlobalSettings.MenuBarButtonStatus.IsVacationCorrection || GlobalSettings.MenuBarButtonStatus.IsEOM) {
                foreach (var item in GlobalSettings.WBidINIContent.BidLineVacationColumns) {
                    var col = GlobalSettings.BidlineAdditionalvacationColumns.FirstOrDefault (x => x.Id == item);
                    if (col != null)
                        CommonClass.bidLineProperties.Add (col.DisplayName);
                }
                foreach (var item in GlobalSettings.WBidINIContent.ModernVacationColumns) {
                    var col = GlobalSettings.ModernAdditionalvacationColumns.FirstOrDefault (x => x.Id == item);
                    if (col != null)
                        CommonClass.modernProperties.Add (col.DisplayName);
                }

            } else {
                foreach (var item in GlobalSettings.WBidINIContent.BidLineNormalColumns) {
                    var col = GlobalSettings.BidlineAdditionalColumns.FirstOrDefault (x => x.Id == item);
                    if (col != null)
                        CommonClass.bidLineProperties.Add (col.DisplayName);
                }
                foreach (var item in GlobalSettings.WBidINIContent.ModernNormalColumns) {
                    var col = GlobalSettings.ModernAdditionalColumns.FirstOrDefault (x => x.Id == item);
                    if (col != null)
                        CommonClass.modernProperties.Add (col.DisplayName);
                }

            }

//            if (GlobalSettings.MenuBarButtonStatus.IsVacationCorrection || GlobalSettings.MenuBarButtonStatus.IsEOM) {
//                CommonClass.modernProperties = new List<string> () {
//                    "TotPay",
//                    "VacPay",
//                    "FlyPay",
//                    "Off",
//                    "+Off"
//                };
////                if (GlobalSettings.ModernAdditionalColumns.Any (x => x.DisplayName == "Pay"))
////                    GlobalSettings.ModernAdditionalColumns.FirstOrDefault (x => x.DisplayName == "Pay").DisplayName = "TotPay";
//            } else {
//                CommonClass.modernProperties = new List<string> () {
//                    "Pay",
//                    "PDiem",
//                    "Flt",
//                    "Off",
//                    "+Off"
//                };
////                if (GlobalSettings.ModernAdditionalColumns.Any (x => x.DisplayName == "TotPay"))
////                    GlobalSettings.ModernAdditionalColumns.FirstOrDefault (x => x.DisplayName == "TotPay").DisplayName = "Pay";
//            }

        }

        void RedoOperation ()
        {
            if (GlobalSettings.RedoStack.Count > 0) {
                var state = GlobalSettings.RedoStack [0];

                bool isNeedtoRecreateMILFile = false;
                if (state.MILDateList != null && wBIdStateContent.MILDateList != null)
                    isNeedtoRecreateMILFile = checkToRecreateMILFile (state.MILDateList, wBIdStateContent.MILDateList);

                StateManagement stateManagement = new StateManagement ();
                stateManagement.UpdateWBidStateContent ();

                var stateContent = GlobalSettings.WBidStateCollection.StateList.FirstOrDefault (x => x.StateName == state.StateName);

                if (stateContent != null) {
                    GlobalSettings.UndoStack.Insert (0, new WBidState (stateContent));
                    GlobalSettings.WBidStateCollection.StateList.Remove (stateContent);
                    GlobalSettings.WBidStateCollection.StateList.Insert (0, new WBidState (state));

                }

                GlobalSettings.RedoStack.RemoveAt (0);

                if (isNeedtoRecreateMILFile) {
                    GlobalSettings.MILDates = WBidCollection.GenarateOrderedMILDates (wBIdStateContent.MILDateList);
                    GlobalSettings.MILData = CreateNewMILFile ().MILValue;

                }
                //   StateManagement stateManagement = new StateManagement();
                //stateManagement.ReloadDataFromStateFile();
                bool isNeedToRecalculateLineProp = stateManagement.CheckLinePropertiesNeedToRecalculate (state);
                ResetLinePropertiesBackToNormal (stateContent, state);
                ResetOverlapState (stateContent, state);

                //Setting Button status to Global variables
                stateManagement.SetMenuBarButtonStatusFromStateFile (state);
                //Setting  status to Global variables
                stateManagement.SetVacationOrOverlapExists (state);

                SetVacButtonStates ();

                if (isNeedToRecalculateLineProp) {
                    overlayPanel = new NSPanel ();
                    overlayPanel.SetContentSize (new CGSize (400, 120));
                    overlay = new OverlayViewController ();
                    overlay.OverlayText = "Please wait..";
                    overlayPanel.ContentView = overlay.View;
                    NSApplication.SharedApplication.BeginSheet (overlayPanel, this.Window);
                    BeginInvokeOnMainThread (() => {

                        stateManagement.RecalculateLineProperties (state);
                        InvokeOnMainThread (() => {
                            NSApplication.SharedApplication.EndSheet (overlayPanel);
                            overlayPanel.OrderOut (this);
                            stateManagement.ReloadStateContent (state);
                            ReloadAllContent ();
                            if (CommonClass.CSWController != null) {
                                CommonClass.CSWController.ReloadAllContent ();
                            }
                        });

                    });


                } else {
                    stateManagement.ReloadStateContent (state);
                    ReloadAllContent ();
                    if (CommonClass.CSWController != null) {
                        CommonClass.CSWController.ReloadAllContent ();
                    }
                }

            }

            GlobalSettings.isUndo = false;
            GlobalSettings.isRedo = true;
            UpdateUndoRedoButtons ();
            GlobalSettings.isModified = true;
            btnSave.Enabled = GlobalSettings.isModified;

        }

        private bool checkToRecreateMILFile (List<Absense> lstPreviosusMIL, List<Absense> lstCurrentMIL)
        {
            bool isNeedtoReCreateMILFile = false;
            if (lstPreviosusMIL.Count != lstCurrentMIL.Count)
                isNeedtoReCreateMILFile = true;
            else {
                for (int count = 0; count < lstPreviosusMIL.Count; count++) {
                    if (lstPreviosusMIL [count].StartAbsenceDate != lstCurrentMIL [count].StartAbsenceDate || lstPreviosusMIL [count].EndAbsenceDate != lstCurrentMIL [count].EndAbsenceDate) {
                        isNeedtoReCreateMILFile = true;
                        break;
                    }

                }
            }
            return isNeedtoReCreateMILFile;
        }

        void UndoOperation ()
        {
            if (GlobalSettings.UndoStack.Count > 0) {
                WBidState state = GlobalSettings.UndoStack [0];


                bool isNeedtoRecreateMILFile = false;
                if (state.MILDateList != null && wBIdStateContent.MILDateList != null)
                    isNeedtoRecreateMILFile = checkToRecreateMILFile (state.MILDateList, wBIdStateContent.MILDateList);

                StateManagement stateManagement = new StateManagement ();
                stateManagement.UpdateWBidStateContent ();

                var stateContent = GlobalSettings.WBidStateCollection.StateList.FirstOrDefault (x => x.StateName == state.StateName);
            

                if (stateContent != null) {
                    GlobalSettings.RedoStack.Insert (0, new WBidState (stateContent));
                    GlobalSettings.WBidStateCollection.StateList.Remove (stateContent);
                    GlobalSettings.WBidStateCollection.StateList.Insert (0, new WBidState (state));

                }


                GlobalSettings.UndoStack.RemoveAt (0);

                if (isNeedtoRecreateMILFile) {
                    GlobalSettings.MILDates = WBidCollection.GenarateOrderedMILDates (wBIdStateContent.MILDateList);
                    GlobalSettings.MILData = CreateNewMILFile ().MILValue;

                }
                bool isNeedToRecalculateLineProp = stateManagement.CheckLinePropertiesNeedToRecalculate (state);
                ResetLinePropertiesBackToNormal (stateContent, state);
                ResetOverlapState (stateContent, state);

                //Setting Button status to Global variables
                stateManagement.SetMenuBarButtonStatusFromStateFile (state);
                //Setting  status to Global variables
                stateManagement.SetVacationOrOverlapExists (state);

                SetVacButtonStates ();

                if (isNeedToRecalculateLineProp) {
                    overlayPanel = new NSPanel ();
                    overlayPanel.SetContentSize (new CGSize (400, 120));
                    overlay = new OverlayViewController ();
                    overlay.OverlayText = "Please wait..";
                    overlayPanel.ContentView = overlay.View;
                    NSApplication.SharedApplication.BeginSheet (overlayPanel, this.Window);
                    BeginInvokeOnMainThread (() => {

                        stateManagement.RecalculateLineProperties (state);
                        InvokeOnMainThread (() => {
                            NSApplication.SharedApplication.EndSheet (overlayPanel);
                            overlayPanel.OrderOut (this);
                            stateManagement.ReloadStateContent (state);
                            ReloadAllContent ();
                            if (CommonClass.CSWController != null) {
                                CommonClass.CSWController.ReloadAllContent ();
                            }
                        });

                    });


                } else {
                    stateManagement.ReloadStateContent (state);
                    ReloadAllContent ();
                    if (CommonClass.CSWController != null) {
                        CommonClass.CSWController.ReloadAllContent ();
                    }
                }
                            
                        


                //stateManagement.ReloadDataFromStateFile();
                //ReloadLineView ();


            }

            GlobalSettings.isUndo = true;
            GlobalSettings.isRedo = false;
            UpdateUndoRedoButtons ();
            GlobalSettings.isModified = true;
            btnSave.Enabled = GlobalSettings.isModified;
        }

        public void UpdateUndoRedoButtons ()
        {
            btnUndo.Title = GlobalSettings.UndoStack.Count.ToString ();
            btnRedo.Title = GlobalSettings.RedoStack.Count.ToString ();

//            if (GlobalSettings.isUndo)
//                btnUndo.Image = NSImage.ImageNamed ("undoOrange.png");
//            else
//                btnUndo.Image = NSImage.ImageNamed ("undoGreen.png");
//
//            if (GlobalSettings.isRedo)
//                btnRedo.Image = NSImage.ImageNamed ("redoOrange.png");
//            else
//                btnRedo.Image = NSImage.ImageNamed ("redoGreen.png");

            if (GlobalSettings.UndoStack.Count == 0) {
//                btnUndo.Image = NSImage.ImageNamed ("undoGreen.png");
                btnUndo.Title = string.Empty;
                btnUndo.Enabled = false;
            } else {
                btnUndo.Title = GlobalSettings.UndoStack.Count.ToString ();
                btnUndo.Enabled = true;
            }

            if (GlobalSettings.RedoStack.Count == 0) {
//                btnRedo.Image = NSImage.ImageNamed ("redoGreen.png");
                btnRedo.Title = string.Empty;
                btnRedo.Enabled = false;
            } else {
                btnRedo.Title = GlobalSettings.RedoStack.Count.ToString ();
                btnRedo.Enabled = true;
            }

            GlobalSettings.isUndo = false;
            GlobalSettings.isRedo = false;
        }

        public void UpdateSaveButton (bool value)
        {
            try {
                GlobalSettings.isModified = value;
                btnSave.Enabled = GlobalSettings.isModified;
            } catch (Exception ex) {
                CommonClass.AppDelegate.ErrorLog (ex);
                CommonClass.AppDelegate.ShowErrorMessage (WBidErrorMessages.CommonError);
            }
        }

        void SetupAdminView ()
        {
            //txtUser.Enabled = false;
            btnReparse.Activated += HandleReparse;
            btnSecretDownload.Activated += HandleSecretDownload;
            txtUser.StringValue = GlobalSettings.ModifiedEmployeeNumber ?? string.Empty;
            swUser.State = GlobalSettings.IsDifferentUser ? NSCellStateValue.On : NSCellStateValue.Off;
            swSeniority.State=GlobalSettings.IsNeedToDownloadSeniority ? NSCellStateValue.On : NSCellStateValue.Off;
            swUser.Activated += (object sender, EventArgs e) => {
                //txtUser.Enabled = (swUser.State == NSCellStateValue.On);
                GlobalSettings.IsDifferentUser = (swUser.State == NSCellStateValue.On);
                //GlobalSettings.ModifiedEmployeeNumber =txtUser.StringValue;

            };

            swSeniority.Activated+=(object sender, EventArgs e) => {
                GlobalSettings.IsNeedToDownloadSeniority = (swSeniority.State == NSCellStateValue.On);
            };
            txtUser.Changed += (object sender, EventArgs e) => {
                GlobalSettings.ModifiedEmployeeNumber = txtUser.StringValue;
            };

        }

        public void CloseAllChildWindows ()
        {
            if (this.Window.ChildWindows != null) {
                foreach (var item in this.Window.ChildWindows) {
                    item.Close ();
                }
            }
        }

        void HandleGoToLine (object sender, EventArgs e)
        {
            try {
                if (txtGoToLine.StringValue != string.Empty) {
                    var num = txtGoToLine.IntValue;//int.Parse (txtGoToLine.IntValue);
                    if (GlobalSettings.Lines.Any (x => x.LineNum == num)) {
                        var line = GlobalSettings.Lines.FirstOrDefault (x => x.LineNum == num);
                        if (sgViewSelect.SelectedSegment == 0)
                            CommonClass.SummaryController.GoToLine (line);
                        else if (sgViewSelect.SelectedSegment == 1)
                            CommonClass.BidLineController.GoToLine (line);
                        else if (sgViewSelect.SelectedSegment == 2)
                            CommonClass.ModernController.GoToLine (line);
                    }
                }
            } catch (Exception ex) {
                CommonClass.AppDelegate.ErrorLog (ex);
                CommonClass.AppDelegate.ShowErrorMessage (WBidErrorMessages.CommonError);
            }
        }


        

        private void setViews ()
        {
            btnTopLock.Image = NSImage.ImageNamed ("topLockGreen.png");
            btnBottomLock.Image = NSImage.ImageNamed ("bottomLockRed.png");
            btnRemTopLock.Image = NSImage.ImageNamed ("removeLockGreen.png");
            btnRemBottomLock.Image = NSImage.ImageNamed ("removeLockRed.png");
            var interfaceStyle = NSUserDefaults.StandardUserDefaults.StringForKey("AppleInterfaceStyle");
            if (interfaceStyle == "Dark") {
                btnHome.Image = NSImage.ImageNamed("homeMacDark.png");
                btnSave.Image = NSImage.ImageNamed("saveMacDark.png");
                btnSynch.Image = NSImage.ImageNamed("synchIconDark.png");
            }
            else {
                btnHome.Image = NSImage.ImageNamed("homeMac.png");
                btnSave.Image = NSImage.ImageNamed("saveMac.png");
                btnSynch.Image = NSImage.ImageNamed("synchIcon3.png");
            }

            

            //MonoMac.CoreGraphics.CGColor aa = new MonoMac.CoreGraphics.CGColor ("Red");
            //okbtnVacation.Layer.BackgroundColor=new MonoMac.CoreGraphics.CGColor(254/255f,200/255f,200/255f);
            //btnVacation.WantsLayer = true;
            //btnVacation.Layer.BackgroundColor=NSColor.Red.CGColor;
            //btnVacation.SetUpGState ();;
            //btnEOM.Cell.ControlTint = NSControlTint.Blue;

            //btnCSW.Cell.BackgroundColor = NSColor.Red;
            //btnCSW.Bordered = false;
            //btnEOM.Cell.BackgroundColor = NSColor.Red;
            //btnEOM.WantsLayer = true;
            //btnEOM.Bordered = false;

            sgViewSelect.Activated += HandleViewSelect;
            btnTopLock.Activated += btnTopLockClicked;
            btnBottomLock.Activated += btnBottomLockClicked;
            btnRemTopLock.Activated += btnRemTopLockClicked;
            btnRemBottomLock.Activated += btnRemBottomLockClicked;
            btnHome.Activated += btnHomeClicked;
            btnSave.Activated += btnSaveClicked;
            btnCSW.Activated += btnCSWClicked;
            btnBA.Activated += btnBAClicked;
            btnOverlap.Activated += btnOverLapClicked;
            btnVacation.Activated += btnVacationClicked;
            btnDrop.Activated += btnVacationDropClicked;
            btnEOM.Activated += btnEOMClicked;
            btnReset.Activated += btnResetClicked;
            btnSynch.Activated += btnSynchClicked;
            btnQuickSet.Activated += btnQuickSetClicked;
            btnMIL.Activated += btnMILClicked;
            btnPairings.Activated += btnPairingsClicked;
            btnVacDiff.Activated += btnVacDiffClicked;
            btnSynch.Enabled = GlobalSettings.WBidINIContent.User.SmartSynch;
            btnMIL.Hidden = !GlobalSettings.WBidINIContent.User.MIL;

            if (GlobalSettings.WBidINIContent.ViewType == 0 || GlobalSettings.WBidINIContent.ViewType == 1)
                sgViewSelect.SetSelected (true, 0);
            else if (GlobalSettings.WBidINIContent.ViewType == 2)
                sgViewSelect.SetSelected (true, 1);
            else if (GlobalSettings.WBidINIContent.ViewType == 3)
                sgViewSelect.SetSelected (true, 2);
            ChangeView ();
        }

        public void ToggleView (int index)
        {
            CommonClass.ViewChanged = true;
            GlobalSettings.WBidINIContent.ViewType = index + 1;
            sgViewSelect.SetSelected (true, index);
            ChangeView ();
            ReloadAllContent ();
            CommonClass.ViewChanged = false;
        }

        void btnPairingsClicked (object sender, EventArgs e)
        {
            pairing = new PairingWindowController ();
            //this.Window.AddChildWindow (pairing.Window, NSWindowOrderingMode.Above);
            pairing.Window.MakeKeyAndOrderFront(this);
            pairing.Window.Level = NSWindowLevel.Floating;
            pairing.ShowWindow(this);

            //NSApplication.SharedApplication.RunModalForWindow (pairing.Window);
        }
        void btnVacDiffClicked(object sender, EventArgs e)
        {
            if (GlobalSettings.CurrentBidDetails.Postion == "FA")
            {
                //show commut diff.
                var showcmtDiff = new CommutDifferenceControllerController();
                showcmtDiff.Window.MakeKeyAndOrderFront(this);
                showcmtDiff.GetCommuteDifferenceData();

                if (!showcmtDiff.IsNeedToClose)
                {
                    CommonClass.MainController.Window.AddChildWindow(showcmtDiff.Window, NSWindowOrderingMode.Above);
                    NSApplication.SharedApplication.RunModalForWindow(showcmtDiff.Window);
                }
            }
            else
            {
                bool isCommuteAutoAvailable = IsCommuteAutoAvailable();
                
                if (((isCommuteAutoAvailable && (GlobalSettings.ServerFlightDataVersion != GlobalSettings.WBidINIContent.LocalFlightDataVersion)) && (btnVacation.Enabled || btnEOM.Enabled)))
                {
                    var alert = new NSAlert();
                    alert.AlertStyle = NSAlertStyle.Informational;
                    alert.MessageText = "WBidMax";
                    alert.InformativeText = "You may have changes in commute and vacation due to new flight data. What would you like to open?";
                    alert.AddButton("View Vacation Difference");
                    alert.AddButton("View Commute Difference");
                    alert.Buttons[0].Activated += (object sender1, EventArgs ex) =>
                    {
                        alert.Window.Close();
                        NSApplication.SharedApplication.StopModal();

                        var showvacDiff = new VacationDifferenceControllerController();
                        showvacDiff.Window.MakeKeyAndOrderFront(this);
                        showvacDiff.GetVacationDifffrenceData();
                        if (!showvacDiff.IsNeedToClose)
                        {
                            CommonClass.MainController.Window.AddChildWindow(showvacDiff.Window, NSWindowOrderingMode.Above);
                            NSApplication.SharedApplication.RunModalForWindow(showvacDiff.Window);
                        }
                    };
                    alert.Buttons[1].Activated += (object sender1, EventArgs ex) =>
                    {
                        alert.Window.Close();
                        NSApplication.SharedApplication.StopModal();

                        var showcmtDiff = new CommutDifferenceControllerController();
                        showcmtDiff.Window.MakeKeyAndOrderFront(this);
                        showcmtDiff.GetCommuteDifferenceData();

                        if (!showcmtDiff.IsNeedToClose)
                        {
                            CommonClass.MainController.Window.AddChildWindow(showcmtDiff.Window, NSWindowOrderingMode.Above);
                            NSApplication.SharedApplication.RunModalForWindow(showcmtDiff.Window);
                        }
                    };
                    alert.RunModal();
                }
                else if ((isCommuteAutoAvailable && (GlobalSettings.ServerFlightDataVersion != GlobalSettings.WBidINIContent.LocalFlightDataVersion)))
                {
                    //show commut diff

                    var showcmtDiff = new CommutDifferenceControllerController();
                    showcmtDiff.Window.MakeKeyAndOrderFront(this);
                    showcmtDiff.GetCommuteDifferenceData();

                    if (!showcmtDiff.IsNeedToClose)
                    {
                        CommonClass.MainController.Window.AddChildWindow(showcmtDiff.Window, NSWindowOrderingMode.Above);
                        NSApplication.SharedApplication.RunModalForWindow(showcmtDiff.Window);
                    }
                }
                else
                {
                    //show vac diff
                    var showvacDiff = new VacationDifferenceControllerController();
                    showvacDiff.Window.MakeKeyAndOrderFront(this);
                    showvacDiff.GetVacationDifffrenceData();
                    if (!showvacDiff.IsNeedToClose)
                    {
                        CommonClass.MainController.Window.AddChildWindow(showvacDiff.Window, NSWindowOrderingMode.Above);
                        NSApplication.SharedApplication.RunModalForWindow(showvacDiff.Window);
                    }
                }

            }
        }
        void btnMILClicked (object sender, EventArgs e)
        {
            if (btnMIL.State == NSCellStateValue.On) {
                var panel = new NSPanel ();
                var milConf = new MILConfigViewController ();
                CommonClass.MILController = milConf;
                CommonClass.Panel = panel;
                panel.SetContentSize (new CGSize (500, 300));
                panel.ContentView = milConf.View;
                NSApplication.SharedApplication.BeginSheet (panel, CommonClass.MainController.Window);
            } else {
                WBidHelper.PushToUndoStack ();
                UpdateUndoRedoButtons ();
//                LoadingOverlay overlay = new LoadingOverlay (this.View.Frame, "Removing MIL. Please wait.. ");
//                this.View.Add (overlay);
                overlayPanel = new NSPanel ();
                overlayPanel.SetContentSize (new CGSize (400, 120));
                overlay = new OverlayViewController ();
                overlay.OverlayText = "Removing MIL Data..";
                overlayPanel.ContentView = overlay.View;
                NSApplication.SharedApplication.BeginSheet (overlayPanel, this.Window);

                BeginInvokeOnMainThread (() => {
                    GlobalSettings.MenuBarButtonStatus.IsMIL = false;

//                    RecalcalculateLineProperties RecalcalculateLineProperties = new RecalcalculateLineProperties ();
//                    RecalcalculateLineProperties.CalcalculateLineProperties ();
//                    PrepareModernBidLineView prepareModernBidLineView = new PrepareModernBidLineView ();
//                    prepareModernBidLineView.CalculatebidLinePropertiesforVacation ();

                    StateManagement statemanagement = new StateManagement ();
                    WBidState wBidStateCont = GlobalSettings.WBidStateCollection.StateList.FirstOrDefault (x => x.StateName == GlobalSettings.WBidStateCollection.DefaultName);
                    statemanagement.RecalculateLineProperties (wBidStateCont);
                    statemanagement.ApplyCSW (wBidStateCont);
                    //SortLineList ();
                    InvokeOnMainThread (() => {
                        NSApplication.SharedApplication.EndSheet (overlayPanel);
                        overlayPanel.OrderOut (this);
                        UpdateSaveButton (true);
                        SetVacButtonStates ();
                        ReloadAllContent ();
                    });
                });

            }

            var wBidStateContent = GlobalSettings.WBidStateCollection.StateList.FirstOrDefault (x => x.StateName == GlobalSettings.WBidStateCollection.DefaultName);
            wBidStateContent.MenuBarButtonState.IsMIL = GlobalSettings.MenuBarButtonStatus.IsMIL;
        }

        void btnQuickSetClicked (object sender, EventArgs e)
        {
             qsWC = new QuickSetWindowController ();
            //this.Window.AddChildWindow (qsWC.Window, NSWindowOrderingMode.Above);
            //NSApplication.SharedApplication.RunModalForWindow (qsWC.Window);
            qsWC.Window.MakeKeyAndOrderFront (this);
            qsWC.Window.Level = NSWindowLevel.Floating;
            qsWC.ShowWindow(this);
        }

        void btnSynchClicked (object sender, EventArgs e)
        {
            if (GlobalSettings.isModified) {
                //UIAlertView syAlert = new UIAlertView("Smart Sync", "Please save the current state before performing synch.", null, "OK", null);
                //syAlert.Show();
                ShowMessageBox ("Smart Sync", "Please save the current state before performing synch.");
            } else {
                BeginInvokeOnMainThread (() => {
                    SynchBtn = true;
                    Synch ();
                });
            }

        }

        void btnResetClicked (object sender, EventArgs e)
        {
            ResetAll ();
        }

        private void HandleReparse (object sender, EventArgs e)
        {
            if (swUser.State == NSCellStateValue.On) {
                GlobalSettings.IsDifferentUser = true;
                GlobalSettings.ModifiedEmployeeNumber = txtUser.StringValue;
            }

            var alert = new NSAlert ();
            alert.MessageText = "WBidMax";
            alert.InformativeText = "Do you want to test Vacation Correction?";
            alert.AddButton ("YES");
            alert.AddButton ("No");
            alert.Buttons [0].Activated += (object senderr, EventArgs ee) => {
                alert.Window.Close ();
                NSApplication.SharedApplication.StopModal ();
                // show test vacation view
                var panel = new NSPanel ();
                var testVac = new TestVacationViewController ();
                CommonClass.Panel = panel;
                panel.SetContentSize (new CGSize (350, 726));
                panel.ContentView = testVac.View;
                NSApplication.SharedApplication.BeginSheet (panel, CommonClass.MainController.Window);

                btnDrop.State = NSCellStateValue.Off;
                btnVacation.State = NSCellStateValue.Off;

            };
            alert.Buttons [1].Activated += (object senderr, EventArgs ee) => {
                alert.Window.Close ();
                NSApplication.SharedApplication.StopModal ();
                // reparse
                PerformReparse ();
            };
            alert.RunModal ();

//            //if (btnReparseCheck.Selected)
//            //{
//            //    GlobalSettings.IsDifferentUser = true;
//            //    GlobalSettings.ModifiedEmployeeNumber = txtReparse.Text;
//            //}
//            //if (e.ButtonIndex == 0)
//            //{
//            //    var loadingOverlay = new LoadingOverlay(View.Bounds, "Reparsing..Please Wait..");
//            //    View.Add(loadingOverlay);
//            //    InvokeInBackground(() =>
//            //        {
//                        string zipFilename = WBidHelper.GenarateZipFileName();
//                        ReparseParameters reparseParams = new ReparseParameters() { ZipFileName = zipFilename };
//                        ReparseBL.ReparseTripAndLineFiles(reparseParams);
//                        
//            //        });
//            //}
//            //else if (//vacation corection)
//            //{
        }

        public void PerformReparse ()
        {
            GlobalSettings.MenuBarButtonStatus.IsEOM = false;
            GlobalSettings.MenuBarButtonStatus.IsOverlap = false;
            GlobalSettings.MenuBarButtonStatus.IsVacationCorrection = false;
            GlobalSettings.MenuBarButtonStatus.IsVacationDrop = false;
            overlayPanel = new NSPanel ();
            overlayPanel.SetContentSize (new CGSize (400, 120));
            overlay = new OverlayViewController ();
            overlay.OverlayText = "Reparsing.. Please wait.";
            overlayPanel.ContentView = overlay.View;
            NSApplication.SharedApplication.BeginSheet (overlayPanel, this.Window);

            string zipFilename = WBidHelper.GenarateZipFileName ();
            ReparseParameters reparseParams = new ReparseParameters () { ZipFileName = zipFilename };
            WBidState wBIdStateContent = GlobalSettings.WBidStateCollection.StateList.FirstOrDefault (x => x.StateName == GlobalSettings.WBidStateCollection.DefaultName);
            string fileToSave = WBidHelper.GenerateFileNameUsingCurrentBidDetails ();
            if (wBIdStateContent.IsOverlapCorrection) {

                if (File.Exists (WBidHelper.GetAppDataPath () + "/" + fileToSave + ".OL")) {
                    // OverlapData overlapData = XmlHelper.DeserializeFromXml<OverlapData>(WBidHelper.GetAppDataPath() + "/" + fileToSave + ".OL");
                    OverlapData overlapData;
                    using (FileStream filestream = File.OpenRead (WBidHelper.GetAppDataPath () + "/" + fileToSave + ".OL")) {

                        OverlapData overlapdataobj = new OverlapData ();
                        overlapData = ProtoSerailizer.DeSerializeObject (WBidHelper.GetAppDataPath () + "/" + fileToSave + ".OL", overlapdataobj, filestream);
                    }

                    if (overlapData != null) {
                        GlobalSettings.LeadOutDays = overlapData.LeadOutDays;
                        GlobalSettings.LastLegArrivalTime = Convert.ToInt32 (overlapData.LastLegArrivalTime);
                    }
                }
            }


            ReparseBL.ReparseTripAndLineFiles (reparseParams);
            string currentBidName = WBidHelper.GenerateFileNameUsingCurrentBidDetails ();
            string vACFileName = WBidHelper.GetAppDataPath () + "/" + currentBidName + ".VAC";
            Dictionary<string, TripMultiVacData> VacationData;
            if (GlobalSettings.IsVacationCorrection) {
                bool vacFileExists = File.Exists (vACFileName);
                //if the vac file  for EOM already exists and user doesnot have any vacation , we need to overwrite the vac file 
                if (!wBIdStateContent.IsVacationOverlapOverlapCorrection) {
                    vacFileExists = false;

                }
            
                wBIdStateContent.IsVacationOverlapOverlapCorrection = GlobalSettings.IsVacationCorrection;
                if (!vacFileExists) {
                    VacationData = new Dictionary<string, TripMultiVacData> ();
                } else {

                    using (FileStream vacstream = File.OpenRead (vACFileName)) {

                        Dictionary<string, TripMultiVacData> objineinfo = new Dictionary<string, TripMultiVacData> ();
                        VacationData = ProtoSerailizer.DeSerializeObject (vACFileName, objineinfo, vacstream);

                    }
                }

                if (GlobalSettings.SeniorityListMember != null && GlobalSettings.SeniorityListMember.Absences != null && GlobalSettings.SeniorityListMember.Absences.Where (y => y.AbsenceType == "VA").Count () > 0 && File.Exists (vACFileName)) {

                    CaculateVacationDetails caculateVacationDetails = new CaculateVacationDetails ();
                    //caculateVacationDetails.CalculateVacationdetailsFromVACfile (lines, vACFileName);
                    lines = GlobalSettings.Lines.ToDictionary (x => x.LineNum.ToString (), x => x);

                    if (GlobalSettings.IsObservedAlgm)
                    {
                        ObserveCaculateVacationDetails observecalVacationdetails = new ObserveCaculateVacationDetails();
                        observecalVacationdetails.CalculateVacationdetailsFromVACfile(lines, VacationData);
                    }
                    else
                    {
                       
                        caculateVacationDetails.CalculateVacationdetailsFromVACfile(lines, VacationData);
                    }
                    
                    // SerializeObject(WBidHelper.GetAppDataPath() + "\\" + filenametosave + ".WBL", lines);

                    GlobalSettings.WBidStateCollection.Vacation = new List<Vacation> ();

                    var vacation = GlobalSettings.SeniorityListMember.Absences.Where (x => x.AbsenceType == "VA").Select (y => new Vacation {
                        StartDate = y.StartAbsenceDate.ToShortDateString (),
                        EndDate = y.EndAbsenceDate.ToShortDateString ()
                    });
                    if (vacation != null) {
                        GlobalSettings.WBidStateCollection.Vacation.AddRange (vacation.ToList ());
                    }
                }
                LineInfo lineInfo = new LineInfo () {
                    LineVersion = GlobalSettings.LineVersion,
                    Lines = lines

                };
                GlobalSettings.Lines = new System.Collections.ObjectModel.ObservableCollection<Line> (lines.Select (x => x.Value));
                var linestream = File.Create (WBidHelper.GetAppDataPath () + "/" + fileToSave + ".WBL");
                ProtoSerailizer.SerializeObject (WBidHelper.GetAppDataPath () + "/" + fileToSave + ".WBL", lineInfo, linestream);
                linestream.Dispose ();
                linestream.Close ();
            }



            NSApplication.SharedApplication.EndSheet (overlayPanel);
            overlayPanel.OrderOut (this);
            SetVacButtonStates ();
            ReloadAllContent ();
        }

        public void PerformVacationReparse ()
        {
            overlayPanel = new NSPanel ();
            overlayPanel.SetContentSize (new CGSize (400, 120));
            overlay = new OverlayViewController ();
            overlay.OverlayText = "Reparsing.. Please wait.";
            overlayPanel.ContentView = overlay.View;
            NSApplication.SharedApplication.BeginSheet (overlayPanel, this.Window);

            string zipFilename = WBidHelper.GenarateZipFileName ();
            ReparseParameters reparseParams = new ReparseParameters () { ZipFileName = zipFilename };
            ReparseLineAndTripFileForvacation (reparseParams);

            NSApplication.SharedApplication.EndSheet (overlayPanel);
            overlayPanel.OrderOut (this);
            SetVacButtonStates ();
            ReloadAllContent ();
        }

        private void ReparseLineAndTripFileForvacation(ReparseParameters reparseParams)
        {


            GlobalSettings.MenuBarButtonStatus.IsEOM = false;
            GlobalSettings.MenuBarButtonStatus.IsOverlap = false;
            GlobalSettings.MenuBarButtonStatus.IsVacationCorrection = false;
            GlobalSettings.MenuBarButtonStatus.IsVacationDrop = false;
            List<string> pairingwHasNoDetails = new List<string>();
            string fileToSave = string.Empty;
            bool isNewAPIFile = false;
            string[] swaProdYearMonth = new string[2];
            swaProdYearMonth = GlobalSettings.SWAProduciton.Split('-');
            DateTime downloadMonthYear = new DateTime(GlobalSettings.CurrentBidDetails.Year, GlobalSettings.CurrentBidDetails.Month, 1);
            DateTime swaMonthYear = new DateTime(Convert.ToInt32(swaProdYearMonth[1]), Convert.ToInt32(swaProdYearMonth[0]), 1);

            if (downloadMonthYear >= swaMonthYear) //pkce date fix
            {
                isNewAPIFile = true;
            }
            if (GlobalSettings.IsSWAApiTest && GlobalSettings.CurrentBidDetails.Postion=="FA" && isNewAPIFile)
            {
                lines = ParseSWALineFile(reparseParams.ZipFileName);
                trips = ParseSWATripFile(reparseParams.ZipFileName,lines);

                //if (reparseParams.ZipFileName.Substring(0, 1) == "A" && reparseParams.ZipFileName.Substring(1, 1) == "B")
                //{

                //    // need to implement second round reparse for pkce
                //}
                //else
                //{
                   
                //}
            }
            else
            {
                //Parse trip and Line file
                trips = ReparseBL.ParseTripFile(reparseParams.ZipFileName);


                if (reparseParams.ZipFileName.Substring(0, 1) == "A" && reparseParams.ZipFileName.Substring(1, 1) == "B")
                {
                    FASecondRoundParser fASecondRound = new FASecondRoundParser();
                    lines = fASecondRound.ParseFASecondRound(WBidHelper.GetAppDataPath() + "/" + reparseParams.ZipFileName.Substring(0, 6).ToString() + "/PS", ref trips, GlobalSettings.FAReserveDayPay, reparseParams.ZipFileName.Substring(2, 3), GlobalSettings.IsOldFormatFAData);

                }
                else
                {
                    lines = ReparseBL.ParseLineFiles(reparseParams.ZipFileName);
                }

            }

            // if (trips == null) return null;

            TripTtpParser tripTtpParser = new TripTtpParser();
            List<CityPair> listCityPair = tripTtpParser.ParseCity(WBidHelper.GetAppDataPath() + "/trips.ttp");
            GlobalSettings.TtpCityPairs = listCityPair;
            //Second Round missing trip management
            //---------------------------------------------------------------------------

            if (GlobalSettings.CurrentBidDetails.Round == "S")
            {   //If  the round is second round ,some times trip list contains  missing trip. So we need  take these trip details from old .WBP file.
                //Otherwise we again need to scrap the missing details from website. The issue is if the bid data is older one, we cannot scrap it from website.
                //tempTrip = reparseParams.Trips;


                //List<string> allPair = lines.SelectMany(x => x.Value.Pairings).ToList();
                //pairingwHasNoDetails = allPair.Where(x => !trips.Select(y => y.Key).ToList().Any(z => z == x.Substring(0, 4))).ToList();
                List<string> allPair = lines.SelectMany(x => x.Value.Pairings).Distinct().ToList();
                pairingwHasNoDetails = allPair.Where(x => !trips.Select(y => y.Key).ToList().Any(z => (z == x.Substring(0, 4)) || (z == x && x.Substring(1, 1) == "P"))).ToList();

                if (pairingwHasNoDetails.Count > 0)
                {
                    Dictionary<string, Trip> missingTrips = new Dictionary<string, Trip>();
                    //missingTrips = missingTrips.Concat(tempTrip.Where(x => pairingwHasNoDetails.Contains(x.Key.ToString()))).ToDictionary(pair => pair.Key, pair => pair.Value);
                    missingTrips = missingTrips.Concat(GlobalSettings.Trip.Where(x => pairingwHasNoDetails.Contains(x.TripNum)).ToDictionary(s => s.TripNum, s => s)).ToDictionary(pair => pair.Key, pair => pair.Value);
                    if (missingTrips.Count == 0)
                    {
                        string bidFileName = string.Empty;
                        bidFileName = GlobalSettings.CurrentBidDetails.Domicile + GlobalSettings.CurrentBidDetails.Postion + "N.TXT";
                        BidLineParser bidLineParser = new BidLineParser();
                        var domcilecode = GlobalSettings.WBidINIContent.Domiciles.FirstOrDefault(x => x.DomicileName == GlobalSettings.CurrentBidDetails.Domicile).Code;
                        missingTrips = missingTrips.Concat(bidLineParser.ParseBidlineFile(WBidHelper.GetAppDataPath() + "\\" + bidFileName, GlobalSettings.CurrentBidDetails.Domicile, domcilecode, GlobalSettings.show1stDay, GlobalSettings.showAfter1stDay, GlobalSettings.CurrentBidDetails.Postion).Where(x => pairingwHasNoDetails.Contains(x.Key))).ToDictionary(pair => pair.Key, pair => pair.Value);
                    }
                    // trips = trips.Concat().ToDictionary(pair => pair.Key, pair => pair.Value);
                    foreach (var trip in missingTrips)
                    {
                        if (!trips.Keys.Contains(trip.Key) && !string.IsNullOrEmpty(trip.Key))
                            trips.Add(trip.Key, trip.Value);
                    }

                }


            }

            //---------------------------------------------------------------------------



            // Additional processing needs to be done to FA trips before CalculateTripPropertyValues
            CalculateTripProperties calcProperties = new CalculateTripProperties();
            if (reparseParams.ZipFileName.Substring(0, 1) == "A")
                calcProperties.PreProcessFaTrips(trips, listCityPair);

            calcProperties.CalculateTripPropertyValues(trips, listCityPair);


            GlobalSettings.Trip = new ObservableCollection<Trip>(trips.Select(x => x.Value));

            CalculateLineProperties calcLineProperties = new CalculateLineProperties();
            bool status=calcLineProperties.CalculateLinePropertyValues(trips, lines, GlobalSettings.CurrentBidDetails);
            if (!status)
                throw new KeyNotFoundException();

            GlobalSettings.Lines = new System.Collections.ObjectModel.ObservableCollection<Line>(lines.Select(x => x.Value));

            if (GlobalSettings.IsVacationCorrection || GlobalSettings.IsFVVacation)
            {
                if (GlobalSettings.IsVacationCorrection)
                {
                    PerformVacation();
                    SaveParsedFiles(trips, lines);


                }
                if (GlobalSettings.IsFVVacation)
                {
                    performFVVacation();
                    WBidHelper.SaveStateFile(WBidHelper.WBidStateFilePath);
                    //SaveParsedFiles(trips, lines);
                }
            }




        }

        /// <summary>
		/// Retrieve SWA Trips
		/// </summary>
		/// <param name="zipFileName"></param>
		/// <returns></returns>
		/// <exception cref="FileNotFoundException"></exception>
        public static Dictionary<string, Trip> ParseSWATripFile(string zipFileName,Dictionary<string,Line>lines)
        {
            string filePath = Path.Combine(WBidHelper.GetAppDataPath(), zipFileName.Substring(0, 6));
            string tripFilePath = Path.Combine(filePath, "SWATripFile.GZP");
            if (!File.Exists(tripFilePath))
                throw new FileNotFoundException("Trip file not found.", tripFilePath);
            using (FileStream fileStream = File.OpenRead(tripFilePath))
            using (GZipStream gzipStream = new GZipStream(fileStream, CompressionMode.Decompress))
            {
                var swaTrips = JsonSerializer.Deserialize<List<SWATrip.LinesPairing>>(gzipStream);
                SwaApiMapper mapper = new SwaApiMapper();
                return mapper.MapSWaTriptoWBidTrip(swaTrips,lines);
            }
        }

        /// <summary>
        /// Retrieve SWA Lines
        /// </summary>
        /// <param name="zipFileName"></param>
        /// <returns></returns>
        /// <exception cref="FileNotFoundException"></exception>
        public static Dictionary<string, Line> ParseSWALineFile(string zipFileName)
        {
            string filePath = Path.Combine(WBidHelper.GetAppDataPath(), zipFileName.Substring(0, 6));
            string tripFilePath = Path.Combine(filePath, "SWALineFile.GZL");
            if (!File.Exists(tripFilePath))
                throw new FileNotFoundException("Line file not found.", tripFilePath);
            using (FileStream fileStream = File.OpenRead(tripFilePath))
            using (GZipStream gzipStream = new GZipStream(fileStream, CompressionMode.Decompress))
            {
                var swaLines = JsonSerializer.Deserialize<List<SWALine.LinesLine>>(gzipStream);
                SwaApiMapper mapper = new SwaApiMapper();
                return mapper.MapSWaLinetoWBidLine(swaLines);
            }
        }

        private void PerformVacation ()
        {
            try {
                VacationCorrectionParams vacationParams = new VacationCorrectionParams ();


                if (GlobalSettings.CurrentBidDetails.Postion != "FA") {
                    string serverPath = GlobalSettings.WBidDownloadFileUrl + "FlightData.zip";
                    string zipLocalFile = Path.Combine (WBidHelper.GetAppDataPath (), "FlightData.zip");
                    string networkDataPath = WBidHelper.GetAppDataPath () + "/" + "FlightData.NDA";

                    FlightPlan flightPlan = null;
                    WebClient wcClient = new WebClient ();
                    //Downloading networkdat file
                    wcClient.DownloadFile (serverPath, zipLocalFile);


                    // Open an existing zip file for reading
                    ZipStorer zip = ZipStorer.Open (zipLocalFile, FileAccess.Read);

                    // Read the central directory collection
                    List<ZipStorer.ZipFileEntry> dir = zip.ReadCentralDir ();

                    // Look for the desired file
                    foreach (ZipStorer.ZipFileEntry entry in dir) {
                        zip.ExtractFile (entry, networkDataPath);
                    }
                    zip.Close ();

                    //Deserializing data to FlightPlan object
                    FlightPlan fp = new FlightPlan ();
                    using (FileStream networkDatatream = File.OpenRead (networkDataPath)) {

                        FlightPlan objineinfo = new FlightPlan ();
                        flightPlan = ProtoSerailizer.DeSerializeObject (networkDataPath, fp, networkDatatream);

                    }

                    if (File.Exists (zipLocalFile)) {
                        File.Delete (zipLocalFile);
                    }
                    if (File.Exists (networkDataPath)) {
                        File.Delete (networkDataPath);
                    }




                    vacationParams.FlightRouteDetails = flightPlan.FlightRoutes.Join (flightPlan.FlightDetails, fr => fr.FlightId, f => f.FlightId,
                        (fr, f) =>
                        new FlightRouteDetails {
                            Flight = f.FlightId,
                            FlightDate = fr.FlightDate,
                            Orig = f.Orig,
                            Dest = f.Dest,
                            Cdep = f.Cdep,
                            Carr = f.Carr,
                            Ldep = f.Ldep,
                            Larr = f.Larr,
                            RouteNum = fr.RouteNum,

                        }).ToList ();

                }


                vacationParams.CurrentBidDetails = GlobalSettings.CurrentBidDetails;
                vacationParams.Trips = trips;
                vacationParams.Lines = lines;
                //  VacationData = new Dictionary<string, TripMultiVacData>();


                //Performing vacation correction algoritham
                VacationCorrectionBL vacationBL = new VacationCorrectionBL ();

                if (GlobalSettings.CurrentBidDetails.Postion != "FA") {
                    if (GlobalSettings.IsObservedAlgm)
                    {
                        ObserveVacationCorrectionBL observevacationBL = new ObserveVacationCorrectionBL();
                        GlobalSettings.VacationData = observevacationBL.PerformVacationCorrection(vacationParams);
                    }
                    else
                    {
                        GlobalSettings.VacationData = vacationBL.PerformVacationCorrection(vacationParams);
                    }
                } else {
                    if (GlobalSettings.IsObservedAlgm)
                    {
                        ObserveVacationCorrectionBL observevacationBL = new ObserveVacationCorrectionBL();
                        GlobalSettings.VacationData = observevacationBL.PerformFAVacationCorrection(vacationParams);
                    }
                    else
                    {
                        GlobalSettings.VacationData = vacationBL.PerformFAVacationCorrection(vacationParams);
                    }
                }



                if (GlobalSettings.VacationData != null) {

                    string fileToSave = string.Empty;
                    fileToSave = WBidHelper.GenerateFileNameUsingCurrentBidDetails ();


                    // save the VAC file to app data folder

                    var stream = File.Create (WBidHelper.GetAppDataPath () + "/" + fileToSave + ".VAC");
                    ProtoSerailizer.SerializeObject (WBidHelper.GetAppDataPath () + "/" + fileToSave + ".VAC", GlobalSettings.VacationData, stream);
                    stream.Dispose ();
                    stream.Close ();
                } else {
                    GlobalSettings.IsVacationCorrection = false;
                }



            } catch (Exception ex) {
                GlobalSettings.IsVacationCorrection = false;
                throw ex;
            }
        }
        private void performFVVacation()
        {
            GlobalSettings.WBidStateCollection.FVVacation = GlobalSettings.FVVacation;
            FVVacation objvac = new FVVacation();
            GlobalSettings.Lines = new ObservableCollection<Line>(objvac.SetFVVacationValuesForAllLines(GlobalSettings.Lines.ToList(), GlobalSettings.VacationData));
        }
        private void SaveParsedFiles (Dictionary<string, Trip> trips, Dictionary<string, Line> lines)
        {

            string fileToSave = string.Empty;

            fileToSave = WBidHelper.GenerateFileNameUsingCurrentBidDetails ();


            TripInfo tripInfo = new TripInfo () {
                TripVersion = GlobalSettings.TripVersion,
                Trips = trips

            };

            var stream = File.Create (WBidHelper.GetAppDataPath () + "/" + fileToSave + ".WBP");
            ProtoSerailizer.SerializeObject (WBidHelper.GetAppDataPath () + "/" + fileToSave + ".WBP", tripInfo, stream);
            stream.Dispose ();
            stream.Close ();

            GlobalSettings.Trip = new ObservableCollection<Trip> (trips.Select (x => x.Value));


            if (GlobalSettings.IsVacationCorrection && GlobalSettings.VacationData != null && GlobalSettings.VacationData.Count > 0) {//set  vacation details  to line object. 

                if (GlobalSettings.IsObservedAlgm)
                {
                    ObserveCaculateVacationDetails observecalVacationdetails = new ObserveCaculateVacationDetails();
                    observecalVacationdetails.CalculateVacationdetailsFromVACfile(lines, GlobalSettings.VacationData);
                }
                else
                {
                    CaculateVacationDetails calVacationdetails = new CaculateVacationDetails();
                    calVacationdetails.CalculateVacationdetailsFromVACfile(lines, GlobalSettings.VacationData);
                }
            }

            LineInfo lineInfo = new LineInfo () {
                LineVersion = GlobalSettings.LineVersion,
                Lines = lines

            };

            GlobalSettings.Lines = new System.Collections.ObjectModel.ObservableCollection<Line> (lines.Select (x => x.Value));

            try {
                var linestream = File.Create (WBidHelper.GetAppDataPath () + "/" + fileToSave + ".WBL");
                ProtoSerailizer.SerializeObject (WBidHelper.GetAppDataPath () + "/" + fileToSave + ".WBL", lineInfo, linestream);
                linestream.Dispose ();
                linestream.Close ();
            } catch (Exception ex) {
                throw ex;
            }


            foreach (Line line in GlobalSettings.Lines) {
                line.ConstraintPoints = new ConstraintPoints ();
                line.WeightPoints = new WeightPoints ();
            }

            //Read the intial state file value from DWC file and create state file
            if (!File.Exists (WBidHelper.GetAppDataPath () + "/" + fileToSave + ".WBS")) {
                try {

                    WBidIntialState wbidintialState = null;
                    try{wbidintialState=XmlHelper.DeserializeFromXml<WBidIntialState> (WBidHelper.GetWBidDWCFilePath ());
                    }catch(Exception ex)
                    {wbidintialState = WBidCollection.CreateDWCFile (GlobalSettings.DwcVersion);
                        XmlHelper.SerializeToXml (wbidintialState, WBidHelper.GetWBidDWCFilePath ());
                        WBidHelper.LogDetails(GlobalSettings.WbidUserContent.UserInformation.EmpNo,"dwcRecreate","0","0");
                        
                    }
                    GlobalSettings.WBidStateCollection = WBidCollection.CreateStateFile (WBidHelper.GetAppDataPath () + "/" + fileToSave + ".WBS", lines.Count, lines.First ().Value.LineNum, wbidintialState);
                } catch (Exception ex) {
                    throw ex;
                }
            } else {
                //Read the state file object and store it to global settings.
                GlobalSettings.WBidStateCollection = null;
                try{GlobalSettings.WBidStateCollection =XmlHelper.DeserializeFromXml<WBidStateCollection> (WBidHelper.GetAppDataPath () + "/" + fileToSave + ".WBS");
                }catch(Exception ex) {


                    WBidIntialState wbidintialState = null;
                    try{wbidintialState=XmlHelper.DeserializeFromXml<WBidIntialState> (WBidHelper.GetWBidDWCFilePath ());
                    }catch(Exception exx)
                    {wbidintialState = WBidCollection.CreateDWCFile (GlobalSettings.DwcVersion);
                        XmlHelper.SerializeToXml (wbidintialState, WBidHelper.GetWBidDWCFilePath ());
                        WBidHelper.LogDetails(GlobalSettings.WbidUserContent.UserInformation.EmpNo,"dwcRecreate","0","0");

                    }

                    GlobalSettings.WBidStateCollection = WBidCollection.CreateStateFile (WBidHelper.GetAppDataPath () + "/" + fileToSave + ".WBS", 400, 1, wbidintialState);
                    WBidHelper.SaveStateFile (fileToSave);
                    WBidHelper.LogDetails(GlobalSettings.WbidUserContent.UserInformation.EmpNo,"wbsRecreate","0","0");
                }
            }
            //save the vacation to state file
            GlobalSettings.WBidStateCollection.Vacation = new List<Vacation> ();
            WBidState wBIdStateContent = GlobalSettings.WBidStateCollection.StateList.FirstOrDefault (x => x.StateName == GlobalSettings.WBidStateCollection.DefaultName);

            if (GlobalSettings.SeniorityListMember != null && GlobalSettings.SeniorityListMember.Absences != null && GlobalSettings.IsVacationCorrection) {
                var vacation = GlobalSettings.SeniorityListMember.Absences.Where (x => x.AbsenceType == "VA").Select (y => new Vacation {
                    StartDate = y.StartAbsenceDate.ToShortDateString (),
                    EndDate = y.EndAbsenceDate.ToShortDateString ()
                });

                GlobalSettings.WBidStateCollection.Vacation.AddRange (vacation.ToList ());

                wBIdStateContent.IsVacationOverlapOverlapCorrection = true;
            } else
                wBIdStateContent.IsVacationOverlapOverlapCorrection = false;
            WBidHelper.SaveStateFile (WBidHelper.WBidStateFilePath);



        }

        public void ShowCSWFromReset()
        {
            try
            {
                cswWC = new CSWWindowController();
                CommonClass.CSWController = cswWC;
                if (!GlobalSettings.WBidINIContent.User.IsCSWViewFloat)
                    cswWC.ShowWindow(this);
                //this.Window.AddChildWindow (cswWC.Window, NSWindowOrderingMode.Above);
                else
                    cswWC.Close();
                //this.Window.RemoveChildWindow (cswWC.Window);
                cswWC.Window.MakeKeyAndOrderFront(this);
                cswWC.Window.Level = NSWindowLevel.Floating;
            }
            catch (Exception ex)
            {
                CommonClass.AppDelegate.ErrorLog(ex);
                CommonClass.AppDelegate.ShowErrorMessage(WBidErrorMessages.CommonError);

            }
        }
        public void ShowCSW ()
        {    
            try {
                if (cswWC == null)
                    cswWC = new CSWWindowController ();
                CommonClass.CSWController = cswWC;
                if (!GlobalSettings.WBidINIContent.User.IsCSWViewFloat)
                    cswWC.ShowWindow(this);
                //this.Window.AddChildWindow (cswWC.Window, NSWindowOrderingMode.Above);
                else
                    cswWC.Close();
                    //this.Window.RemoveChildWindow (cswWC.Window);
                cswWC.Window.MakeKeyAndOrderFront (this);
                cswWC.Window.Level = NSWindowLevel.Floating;
            } catch (Exception ex) {
                CommonClass.AppDelegate.ErrorLog (ex);
                CommonClass.AppDelegate.ShowErrorMessage (WBidErrorMessages.CommonError);
            }

        }

        

        public void ShowBA ()
        {    
            try {
                var wBIdStateContent = GlobalSettings.WBidStateCollection.StateList.FirstOrDefault (x => x.StateName == GlobalSettings.WBidStateCollection.DefaultName);
                if (wBIdStateContent != null && GlobalSettings.CurrentBidDetails != null) {
                    //var anyitemInCSW = CheckIfAnyItemSetInCsw(wBIdStateContent);
                    if (CheckIfAnyItemSetInCsw (wBIdStateContent))
                    {
                        
                            
                        var alert = new NSAlert ();
                        alert.MessageText = "WBidMax";
                        alert.InformativeText = "The Constraints, Top lock, Bottom lock etc from the CSW view will Reset. Do you want to continue to open Bid Automator ?";
                        alert.AddButton ("YES");
                        alert.AddButton ("No");
                        alert.Buttons [0].Activated += (object senderr, EventArgs ee) => {
                            alert.Window.Close ();
                            NSApplication.SharedApplication.StopModal ();

                            StateManagement stateManagement = new StateManagement ();
                            stateManagement.UpdateWBidStateContent ();


                            LineOperations.RemoveAllTopLock ();
                            LineOperations.RemoveAllBottomLock ();
                            CommonClass.selectedRows.Clear ();

                            wBIdStateContent = GlobalSettings.WBidStateCollection.StateList.FirstOrDefault (x => x.StateName == GlobalSettings.WBidStateCollection.DefaultName);
                            wBIdStateContent.SortDetails.SortColumn = "Line";
                            CommonClass.columnID = 0;
                            ConstraintCalculations constCalc = new ConstraintCalculations ();
                            constCalc.ClearConstraints ();
                            ConstraintsApplied.clearAll ();

                            //                    weightCalc.ClearWeights ();
                            //                    WeightsApplied.clearAll ();
                            SortCalculation sort = new SortCalculation ();
                            sort.SortLines ("Line");

                            NSString str = new NSString ("none");
                            NSNotificationCenter.DefaultCenter.PostNotificationName ("ButtonEnableDisable", str);
                            //NSNotificationCenter.DefaultCenter.PostNotificationName ("DataReload", null);
                            CommonClass.MainController.ReloadAllContent ();
                            if(CommonClass.CSWController!=null)
                            {
                            CommonClass.CSWController.ReloadAllContent();
                            }
                            GlobalSettings.isModified = true;
                            InvokeOnMainThread (() => {
                                UpdateSaveButton (false);
                                //UpdateUndoRedoButtons ();
                            });                            
                              this.PerformSelector(new ObjCRuntime.Selector("NavigatetoBA"), null, 0.6);

                        };
                            
                        alert.RunModal();
                    }
                    else
                    {
                        NavigatetoBA();
                    }
                }
            } catch (Exception ex) {
                CommonClass.AppDelegate.ErrorLog (ex);
                CommonClass.AppDelegate.ShowErrorMessage (WBidErrorMessages.CommonError);
            }

        }
        public void ShowSecretDownload()
        {
            try
            {
                if (secretDataView == null)
                    secretDataView = new SecretDataDownloadController();
                CommonClass.SecretDataController = secretDataView;
            //    if (!GlobalSettings.WBidINIContent.User.IsCSWViewFloat)
                    secretDataView.ShowWindow(this);

                //else
                    //secretDataView.Close();
               
                secretDataView.Window.MakeKeyAndOrderFront(this);
                secretDataView.Window.Level = NSWindowLevel.Floating;
            }
            catch (Exception ex)
            {
                CommonClass.AppDelegate.ErrorLog(ex);
                CommonClass.AppDelegate.ShowErrorMessage(WBidErrorMessages.CommonError);
            }

        }
        [Export("NavigatetoBA")]
        private void NavigatetoBA()
        {
            if (baWC == null)
                baWC = new BAWindowController ();
            CommonClass.BAController = baWC;
            if (!GlobalSettings.WBidINIContent.User.IsBAViewFloat)
            {
                baWC.Window.Level = NSWindowLevel.Floating;
                baWC.Window.MakeKeyAndOrderFront(this);
                baWC.ShowWindow(this);
                //this.Window.AddChildWindow(baWC.Window, NSWindowOrderingMode.Above);
            }
            else
            {
                baWC.Window.Close();
                //this.Window.RemoveChildWindow(baWC.Window);
            }

        }

        /// <summary>
        /// this will return true,If anyof the weights,contraints or sorts set
        /// </summary>
        /// <returns></returns>
        private bool CheckIfAnyItemSetInCsw(WBidState wBIdStateContent)
        {

            if (wBIdStateContent.Constraints.Hard)
                return true;
            if (wBIdStateContent.Constraints.Ready)
                return true;
            if (wBIdStateContent.Constraints.Reserve)
                return true;
            if (wBIdStateContent.Constraints.Blank)
                return true;
            if (wBIdStateContent.Constraints.International)
                return true;
            if (wBIdStateContent.Constraints.NonConus)
                return true;
            if (wBIdStateContent.Constraints.ETOPS)
                return true;
            if (wBIdStateContent.CxWtState.ACChg.Cx)
                return true;
            else if (wBIdStateContent.CxWtState.AMPM.Cx)
                return true;
            else if (wBIdStateContent.CxWtState.AMPMMIX.AM)
                return true;
            else if (wBIdStateContent.CxWtState.AMPMMIX.PM)
                return true;
            else if (wBIdStateContent.CxWtState.AMPMMIX.MIX)
                return true;
            else if (wBIdStateContent.CxWtState.BDO.Cx)
                return true;
            else if (wBIdStateContent.CxWtState.BulkOC.Cx)
                return true;
            else if (wBIdStateContent.CxWtState.CL.Cx)
                return true;
            else if (wBIdStateContent.CxWtState.CLAuto.Cx)
                return true;
            else if (wBIdStateContent.CxWtState.DaysOfWeek.SUN)
                return true;
            else if (wBIdStateContent.CxWtState.DaysOfWeek.MON)
                return true;
            else if (wBIdStateContent.CxWtState.DaysOfWeek.TUE)
                return true;
            else if (wBIdStateContent.CxWtState.DaysOfWeek.WED)
                return true;
            else if (wBIdStateContent.CxWtState.DaysOfWeek.THU)
                return true;
            else if (wBIdStateContent.CxWtState.DaysOfWeek.FRI)
                return true;
            else if (wBIdStateContent.CxWtState.DaysOfWeek.SAT)
                return true;
            else if (wBIdStateContent.CxWtState.FaPosition.A)
                return true;
            else if (wBIdStateContent.CxWtState.FaPosition.B)
                return true;
            else if (wBIdStateContent.CxWtState.FaPosition.C)
                return true;
            else if (wBIdStateContent.CxWtState.FaPosition.D)
                return true;
            else if (wBIdStateContent.CxWtState.DHD.Cx)
                return true;
            else if (wBIdStateContent.CxWtState.DHDFoL.Cx)
                return true;
            else if (wBIdStateContent.CxWtState.DOW.Cx)
                return true;
            else if (wBIdStateContent.CxWtState.DP.Cx)
                return true;
            else if (wBIdStateContent.CxWtState.EQUIP.Cx)
                return true;
            else if (wBIdStateContent.CxWtState.FLTMIN.Cx)
                return true;
            else if (wBIdStateContent.CxWtState.GRD.Cx)
                return true;
            else if (wBIdStateContent.CxWtState.InterConus.Cx)
                return true;
            else if (wBIdStateContent.CxWtState.LEGS.Cx)
                return true;
            else if (wBIdStateContent.CxWtState.LegsPerPairing.Cx)
                return true;
            else if (wBIdStateContent.CxWtState.LrgBlkDaysOff.Cx)
                return true;
            else if (wBIdStateContent.CxWtState.MP.Cx)
                return true;
            else if (wBIdStateContent.CxWtState.No3on3off.Cx)
                return true;
            else if (wBIdStateContent.CxWtState.NODO.Cx)
                return true;
            else if (wBIdStateContent.CxWtState.NOL.Cx)
                return true;
            else if (wBIdStateContent.CxWtState.NormalizeDays.Cx)
                return true;
            else if (wBIdStateContent.CxWtState.PDAfter.Cx)
                return true;
            else if (wBIdStateContent.CxWtState.PDBefore.Cx)
                return true;
            else if (wBIdStateContent.CxWtState.PerDiem.Cx)
                return true;
            else if (wBIdStateContent.CxWtState.Position.Cx)
                return true;
            else if (wBIdStateContent.CxWtState.Rest.Cx)
                return true;
            else if (wBIdStateContent.CxWtState.RON.Cx)
                return true;
            else if (wBIdStateContent.CxWtState.SDO.Cx)
                return true;
            else if (wBIdStateContent.CxWtState.SDOW.Cx)
                return true;
            else if (wBIdStateContent.CxWtState.TL.Cx)
                return true;

            else if (wBIdStateContent.CxWtState.WB.Cx)
                return true;
            else if (wBIdStateContent.CxWtState.WorkDay.Cx)
                return true;
            else if (wBIdStateContent.CxWtState.WtPDOFS.Cx)
                return true;
            else if (wBIdStateContent.CxWtState.Commute.Cx)
                return true;
            else if (wBIdStateContent.CxWtState.MixedHardReserveTrip.Cx)
                return true;
            //            //weights
            //
            //            else if (wBIdStateContent.CxWtState.ACChg.Wt)
            //                return true;
            //            else if (wBIdStateContent.CxWtState.AMPM.Wt)
            //                return true;
            //            else if (wBIdStateContent.CxWtState.BDO.Wt)
            //                return true;
            //            else if (wBIdStateContent.CxWtState.BulkOC.Wt)
            //                return true;
            //            else if (wBIdStateContent.CxWtState.CL.Wt)
            //                return true;
            //            else if (wBIdStateContent.CxWtState.DHD.Wt)
            //                return true;
            //            else if (wBIdStateContent.CxWtState.DHDFoL.Wt)
            //                return true;
            //            else if (wBIdStateContent.CxWtState.DOW.Wt)
            //                return true;
            //            else if (wBIdStateContent.CxWtState.DP.Wt)
            //                return true;
            //            else if (wBIdStateContent.CxWtState.EQUIP.Wt)
            //                return true;
            //            else if (wBIdStateContent.CxWtState.FLTMIN.Wt)
            //                return true;
            //            else if (wBIdStateContent.CxWtState.GRD.Wt)
            //                return true;
            //            else if (wBIdStateContent.CxWtState.InterConus.Wt)
            //                return true;
            //            else if (wBIdStateContent.CxWtState.LEGS.Wt)
            //                return true;
            //            else if (wBIdStateContent.CxWtState.LegsPerPairing.Wt)
            //                return true;
            //            else if (wBIdStateContent.CxWtState.LrgBlkDaysOff.Wt)
            //                return true;
            //            else if (wBIdStateContent.CxWtState.MP.Wt)
            //                return true;
            //            else if (wBIdStateContent.CxWtState.No3on3off.Wt)
            //                return true;
            //            else if (wBIdStateContent.CxWtState.NODO.Wt)
            //                return true;
            //            else if (wBIdStateContent.CxWtState.NOL.Wt)
            //                return true;
            //            else if (wBIdStateContent.CxWtState.NormalizeDays.Wt)
            //                return true;
            //            else if (wBIdStateContent.CxWtState.PDAfter.Wt)
            //                return true;
            //            else if (wBIdStateContent.CxWtState.PDBefore.Wt)
            //                return true;
            //            else if (wBIdStateContent.CxWtState.PerDiem.Wt)
            //                return true;
            //            else if (wBIdStateContent.CxWtState.Position.Wt)
            //                return true;
            //            else if (wBIdStateContent.CxWtState.Rest.Wt)
            //                return true;
            //            else if (wBIdStateContent.CxWtState.RON.Wt)
            //                return true;
            //            else if (wBIdStateContent.CxWtState.SDO.Wt)
            //                return true;
            //            else if (wBIdStateContent.CxWtState.SDOW.Wt)
            //                return true;
            //            else if (wBIdStateContent.CxWtState.TL.Wt)
            //                return true;
            //            else if (wBIdStateContent.CxWtState.TripLength.FourDay)
            //                return true;
            //            else if (wBIdStateContent.CxWtState.TripLength.ThreeDay)
            //                return true;
            //            else if (wBIdStateContent.CxWtState.TripLength.Twoday)
            //                return true;
            //            else if (wBIdStateContent.CxWtState.TripLength.Turns)
            //                return true;
            //            else if (wBIdStateContent.CxWtState.WB.Wt)
            //                return true;
            //            else if (wBIdStateContent.CxWtState.WorkDay.Wt)
            //                return true;
            //            else if (wBIdStateContent.CxWtState.WtPDOFS.Wt)
            //                return true;
            //sorts
            else if (wBIdStateContent.SortDetails.SortColumn != "Line")
                return true;
//            else if (GlobalSettings.Lines.Any(x => x.TopLock))
//                return true;
//            else if (GlobalSettings.Lines.Any(x => x.BotLock))
//                return true;
            else
                return false;
        }
        private void applyVacation ()
        {
            try {
                var str = string.Empty;
                if (btnEOM.State == NSCellStateValue.On)
                    str = "Applying EOM";
                else
                    str = "Applying Vacation Correction";

                overlayPanel = new NSPanel ();
                overlayPanel.SetContentSize (new CGSize (400, 120));
                overlay = new OverlayViewController ();
                overlay.OverlayText = str;
                overlayPanel.ContentView = overlay.View;
                NSApplication.SharedApplication.BeginSheet (overlayPanel, this.Window);
                InvokeOnMainThread (() => {
                    try {
                        WBidCollection.GenarateTempAbsenceList ();
                        PrepareModernBidLineView prepareModernBidLineView = new PrepareModernBidLineView ();
                        RecalcalculateLineProperties RecalcalculateLineProperties = new RecalcalculateLineProperties ();
                        prepareModernBidLineView.CalculatebidLinePropertiesforVacation ();
                        RecalcalculateLineProperties.CalcalculateLineProperties ();
                    } catch (Exception ex) {
                        InvokeOnMainThread (() => {
                            throw ex;
                        });
                    }

                    InvokeOnMainThread (() => {
                        NSApplication.SharedApplication.EndSheet (overlayPanel);
                        overlayPanel.OrderOut (this);

                        ReloadAllContent ();
                    });
                });
            } catch (Exception ex) {

                CommonClass.AppDelegate.ErrorLog (ex);
                CommonClass.AppDelegate.ShowErrorMessage (WBidErrorMessages.CommonError);

            }
        }
        private void ShowMonthTomonthAlert()
        {
            
            string AlertText = "";
            var startDateEOM = "";
            var endDateEOM = "";
            var eomstartdate = GetnextSunday();
            var eomEndDate = eomstartdate.AddDays(6);
            List<Weekday> vacationweeks = new List<Weekday>();

            vacationweeks.Add(new Weekday() { StartDate = eomstartdate, EndDate = eomstartdate.AddDays(6), Code = "EOM" });
            startDateEOM = eomstartdate.Day + " " + eomstartdate.ToString("MMM");
            endDateEOM = eomEndDate.Day + " " + eomEndDate.ToString("MMM");
            //EOM Vacation
            AlertText = "You have an 'EOM'  vacation: " + startDateEOM + " - " + endDateEOM;
            AlertText += "\n\nEOM weeks can affect the vacation pay in the current bid period and also the next month.";
            AlertText += "\n\nWe have two documents regarding Month-to-Month vacations that also apply to EOM vacation weeks.";
            AlertText += "\n\nWe suggest you read the following documents to improve your bidding knowledge";

             var monthView = new MonthToMonthAlertViewController();
             monthView.Alert = AlertText;
             CommonClass.MainController.Window.AddChildWindow(monthView.Window, NSWindowOrderingMode.Above);
             NSApplication.SharedApplication.RunModalForWindow(monthView.Window);
            
        }
        void btnEOMClicked (object sender, EventArgs e)
        {
           
            

            try {
               
                WBidHelper.PushToUndoStack ();
                UpdateUndoRedoButtons ();
                if (btnEOM.State == NSCellStateValue.On) {
                    GlobalSettings.MenuBarButtonStatus.IsEOM = true;
                    GlobalSettings.MenuBarButtonStatus.IsVacationDrop = true;
                    btnDrop.Title = "DRP";
                    SetVacButtonStates ();
                   // if (GlobalSettings.CurrentBidDetails.Postion == "FA") {
                        
                        EOMFAViewController eomFA = new EOMFAViewController ();
                        string[] strParams = {
                            String.Format ("{0:m}", GlobalSettings.CurrentBidDetails.BidPeriodEndDate.AddDays (1)),
                            String.Format ("{0:m}", GlobalSettings.CurrentBidDetails.BidPeriodEndDate.AddDays (2)),
                            String.Format ("{0:m}", GlobalSettings.CurrentBidDetails.BidPeriodEndDate.AddDays (3))
                        };
                        eomFA.options = strParams;
                        var panel = new NSPanel ();
                        CommonClass.Panel = panel;
                        panel.SetContentSize (new CGSize (400, 220));
                        panel.ContentView = eomFA.View;
                        NSApplication.SharedApplication.BeginSheet (panel, this.Window);
                        notif = NSNotificationCenter.DefaultCenter.AddObserver ((NSString)"EOMFAVacation", (NSNotification obj) => {
                            NSNotificationCenter.DefaultCenter.RemoveObserver (notif);
                            NSApplication.SharedApplication.EndSheet (CommonClass.Panel);
                            CommonClass.Panel.OrderOut (this);
                            if (obj.Object != null) {
                                //var option = obj.Object.ToString ();
                                handleEOMOptions (Convert.ToInt32 (obj.Object.ToString ()));

                            }
                        });

                    //}
                    //else {
                    //    SetPropertyNames ();
                    //    //sender.Selected = true;
                    //    btnEOM.State = NSCellStateValue.On;


                    //    string currentBidName = WBidHelper.GenerateFileNameUsingCurrentBidDetails ();

                    //    //string zipFileName = GenarateZipFileName();
                    //    string vACFileName = WBidHelper.GetAppDataPath () + "//" + currentBidName + ".VAC";
                    //    //Cheks the VAC file exists
                    //    bool vacFileExists = File.Exists (vACFileName);

                    //    if (!vacFileExists) {

                    //        CreateEOMVacFileForCP (currentBidName);
                    //    } else {



                    //        string overlayTxt = string.Empty;
                    //        if (GlobalSettings.MenuBarButtonStatus.IsEOM)
                    //            overlayTxt = "Applying EOM";
                    //        else
                    //        {
                    //            if (!GlobalSettings.MenuBarButtonStatus.IsVacationCorrection)
                    //            {
                    //                //btnVacDrop.Selected = false;
                    //                btnDrop.State = NSCellStateValue.Off;
                    //                GlobalSettings.MenuBarButtonStatus.IsVacationDrop = false;
                    //            }
                    //            overlayTxt = "Removing EOM";
                    //        }

                    //        overlayPanel = new NSPanel ();
                    //        overlayPanel.SetContentSize (new CGSize (400, 120));
                    //        overlay = new OverlayViewController ();
                    //        overlay.OverlayText = overlayTxt;
                    //        overlayPanel.ContentView = overlay.View;
                    //        NSApplication.SharedApplication.BeginSheet (overlayPanel, this.Window);

                    //        //LoadingOverlay overlay = new LoadingOverlay(this.View.Frame, overlayTxt);
                    //        //this.View.Add(overlay);
                    //        InvokeOnMainThread (() => {

                    //            try {

                    //                if (GlobalSettings.VacationData == null) {
                    //                    using (FileStream vacstream = File.OpenRead (vACFileName)) {

                    //                        Dictionary<string, TripMultiVacData> objineinfo = new Dictionary<string, TripMultiVacData> ();
                    //                        GlobalSettings.VacationData = ProtoSerailizer.DeSerializeObject (vACFileName, objineinfo, vacstream);
                    //                    }
                    //                }


                    //                GenerateVacationDataView ();

                    //                InvokeOnMainThread (() => {
                    //                    //loadSummaryListAndHeader();
                    //                    // NSNotificationCenter.DefaultCenter.PostNotificationName("DataReload", null);
                    //                    NSApplication.SharedApplication.EndSheet (overlayPanel);
                    //                    overlayPanel.OrderOut (this);

                    //                    GlobalSettings.isModified = true;
                    //                    ReloadAllContent ();
                    //                    //CommonClass.lineVC.UpdateSaveButton();
                    //                });
                    //            } catch (Exception ex) {
                    //                InvokeOnMainThread (() => {
                    //                    CommonClass.AppDelegate.ErrorLog (ex);
                    //                    CommonClass.AppDelegate.ShowErrorMessage (WBidErrorMessages.CommonError);
                    //                });
                    //            }
                    //        });

                    //    }




                    //}
                    //if (GlobalSettings.CurrentBidDetails.Postion != "FA")
                    //    ShowMonthTomonthAlert();
                }
                else {

                    btnDrop.Title = "DRP";
                    GlobalSettings.MenuBarButtonStatus.IsEOM = false;
                    GlobalSettings.MenuBarButtonStatus.IsVacationDrop = true;
                    //sender.Selected = false;
                    btnEOM.State = NSCellStateValue.Off;
                    SetPropertyNames ();
                    if (!GlobalSettings.MenuBarButtonStatus.IsVacationCorrection) {
                        //btnVacDrop.Selected = false;
                        btnDrop.State = NSCellStateValue.Off;
                        GlobalSettings.MenuBarButtonStatus.IsVacationDrop = false;
                    }

                    SetVacButtonStates ();

                    string overlayTxt = string.Empty;
                    if (GlobalSettings.MenuBarButtonStatus.IsEOM)
                        overlayTxt = "Applying EOM";
                    else
                    {
                        if (!GlobalSettings.MenuBarButtonStatus.IsVacationCorrection)
                        {
                            //btnVacDrop.Selected = false;
                            btnDrop.State = NSCellStateValue.Off;
                            GlobalSettings.MenuBarButtonStatus.IsVacationDrop = false;
                        }
                        overlayTxt = "Removing EOM";
                    }

                    overlayPanel = new NSPanel ();
                    overlayPanel.SetContentSize (new CGSize (400, 120));
                    overlay = new OverlayViewController ();
                    overlay.OverlayText = overlayTxt;
                    overlayPanel.ContentView = overlay.View;
                    NSApplication.SharedApplication.BeginSheet (overlayPanel, this.Window);

                    InvokeOnMainThread (() => {
                        try {
                            GenerateVacationDataView ();
                        } catch (Exception ex) {
                            InvokeOnMainThread (() => {
                                CommonClass.AppDelegate.ErrorLog (ex);
                                CommonClass.AppDelegate.ShowErrorMessage (WBidErrorMessages.CommonError);
                            });
                        }

                        InvokeOnMainThread (() => {
                            //loadSummaryListAndHeader();
                            //NSNotificationCenter.DefaultCenter.PostNotificationName("DataReload", null);
                            NSApplication.SharedApplication.EndSheet (overlayPanel);
                            overlayPanel.OrderOut (this);

                            GlobalSettings.isModified = true;
                            ReloadAllContent ();
                            //CommonClass.lineVC.UpdateSaveButton();
                        });
                    });

                }

                GlobalSettings.FAEOMStartDate=DateTime.MinValue;
                var wBidStateContent = GlobalSettings.WBidStateCollection.StateList.FirstOrDefault (x => x.StateName == GlobalSettings.WBidStateCollection.DefaultName);
                wBidStateContent.MenuBarButtonState.IsEOM = GlobalSettings.MenuBarButtonStatus.IsEOM;
            } catch (Exception ex) {

                CommonClass.AppDelegate.ErrorLog (ex);
                CommonClass.AppDelegate.ShowErrorMessage (WBidErrorMessages.CommonError);
            }
        }

        private void CreateEOMVacFileForCP ()
        {

            var alert = new NSAlert ();
            alert.AlertStyle = NSAlertStyle.Critical;
            alert.MessageText = "WBidMax";
            alert.InformativeText = "WBidMax needs to download vacation data to make the predictions for your end of month trips (EOM VAC).   This could take up to a minute.  Do you want to continue?";
            alert.AddButton ("YES");
            alert.AddButton ("Cancel");
            alert.Buttons [0].Activated += (object sender, EventArgs e) => {
                alert.Window.Close ();
                NSApplication.SharedApplication.StopModal ();
                DownloadEOMData ();


            };
            alert.Buttons [1].Activated += (object sender, EventArgs e) => {
                if (!wBIdStateContent.IsVacationOverlapOverlapCorrection)
                    btnDrop.Enabled = false;
                btnEOM.State = NSCellStateValue.Off;
                GlobalSettings.MenuBarButtonStatus.IsEOM = false;
                SetVacButtonStates ();
                NSApplication.SharedApplication.StopModal ();
            };
            alert.RunModal ();

        }

        private static void GenerateVacationDataView ()
        {

//            WBidCollection.GenarateTempAbsenceList ();
//            PrepareModernBidLineView prepareModernBidLineView = new PrepareModernBidLineView ();
//            RecalcalculateLineProperties RecalcalculateLineProperties = new RecalcalculateLineProperties ();
//            prepareModernBidLineView.CalculatebidLinePropertiesforVacation ();
//            RecalcalculateLineProperties.CalcalculateLineProperties ();
            //SortLineList ();
            StateManagement statemanagement = new StateManagement ();
            WBidState wBidStateContent = GlobalSettings.WBidStateCollection.StateList.FirstOrDefault (x => x.StateName == GlobalSettings.WBidStateCollection.DefaultName);
            statemanagement.RecalculateLineProperties (wBidStateContent);
            statemanagement.ApplyCSW (wBidStateContent);
        }

        void DownloadEOMData ()
        {

            try {
                if (GlobalSettings.FAEOMStartDate != null && GlobalSettings.FAEOMStartDate != DateTime.MinValue)
                {

                    btnDrop.Enabled = true;
                    string overlayTxt = string.Empty;
                    if (GlobalSettings.MenuBarButtonStatus.IsEOM)
                        overlayTxt = "Applying EOM";
                    else
                    {
                        if (!GlobalSettings.MenuBarButtonStatus.IsVacationCorrection)
                        {
                            //btnVacDrop.Selected = false;
                            btnDrop.State = NSCellStateValue.Off;
                            GlobalSettings.MenuBarButtonStatus.IsVacationDrop = false;
                        }
                        overlayTxt = "Removing EOM";
                    }

                    overlayPanel = new NSPanel();
                    overlayPanel.SetContentSize(new CGSize(400, 120));
                    overlay = new OverlayViewController();
                    overlay.OverlayText = overlayTxt;
                    overlayPanel.ContentView = overlay.View;
                    NSApplication.SharedApplication.BeginSheet(overlayPanel, this.Window);

                    InvokeOnMainThread(() =>
                    {
                        CreateEOMVacationforCP();

                        GenerateVacationDataView();

                        InvokeOnMainThread(() =>
                        {
                            //loadSummaryListAndHeader();

                            NSApplication.SharedApplication.EndSheet(overlayPanel);
                            overlayPanel.OrderOut(this);
                            GlobalSettings.isModified = true;

                            ReloadAllContent();
                        });
                    });
                }

            } catch (Exception ex) {

                CommonClass.AppDelegate.ErrorLog (ex);
                CommonClass.AppDelegate.ShowErrorMessage (WBidErrorMessages.CommonError);
            }
        
        }

        private static void CreateEOMVacationforCP()
        {


            try
            {

                // DateTime nextSunday = GetnextSunday ();

                DateTime eomdate = GlobalSettings.FAEOMStartDate;
                WBidCollection.GenarateTempAbsenceList();
                //GlobalSettings.OrderedVacationDays = new List<Absense>() { new Absense {
                //        StartAbsenceDate = eomdate,
                //        EndAbsenceDate = eomdate.AddDays (6),
                //        AbsenceType = "VA"
                //    }
                //};
                if (GlobalSettings.FlightRouteDetails == null)
                {
                    NetworkData networkplandata = new NetworkData();
                    networkplandata.ReadFlightRoutes();
                }
                //string serverPath = GlobalSettings.WBidDownloadFileUrl + "FlightData.zip";
                //string zipLocalFile = Path.Combine (WBidHelper.GetAppDataPath (), "FlightData.zip");
                //string networkDataPath = WBidHelper.GetAppDataPath () + "/" + "FlightData.NDA";

                //FlightPlan flightPlan = null;
                //WebClient wcClient = new WebClient ();

                //if(File.Exists(networkDataPath))
                //{
                //    File.Delete(networkDataPath);
                //}
                ////Downloading networkdat file
                //wcClient.DownloadFile (serverPath, zipLocalFile);

                ////string appdataPath=WBidHelper.GetAppDataPath();
                ////string target = Path.Combine(appdataPath, WBidHelper.appdataPath + "/" + Path.GetFileNameWithoutExtension(fileName)) + "/";
                ////string zipFile = Path.Combine(appdataPath, fileName);
                ////Extracting the zip file
                ////var zip = new ZipArchive();
                ////zip.EasyUnzip(zipLocalFile, WBidHelper.GetAppDataPath(), true, "");


                //// Open an existing zip file for reading
                //ZipStorer zip = ZipStorer.Open (zipLocalFile, FileAccess.Read);

                //// Read the central directory collection
                //List<ZipStorer.ZipFileEntry> dir = zip.ReadCentralDir ();

                //// Look for the desired file
                //foreach (ZipStorer.ZipFileEntry entry in dir) {
                //    zip.ExtractFile (entry, networkDataPath);
                //}
                //zip.Close ();
                ////Deserializing data to FlightPlan object
                //FlightPlan fp = new FlightPlan ();
                //using (FileStream networkDatatream = File.OpenRead (networkDataPath)) {

                //    FlightPlan objineinfo = new FlightPlan ();
                //    flightPlan = ProtoSerailizer.DeSerializeObject (networkDataPath, fp, networkDatatream);

                //}

                //if (File.Exists (zipLocalFile)) {
                //    File.Delete (zipLocalFile);
                //}
                //if (File.Exists (networkDataPath)) {
                //    File.Delete (networkDataPath);
                //}




                //vacationParams.FlightRouteDetails = flightPlan.FlightRoutes.Join (flightPlan.FlightDetails, fr => fr.FlightId, f => f.FlightId,
                //    (fr, f) =>
                //    new FlightRouteDetails {
                //        Flight = f.FlightId,
                //        FlightDate = fr.FlightDate,
                //        Orig = f.Orig,
                //        Dest = f.Dest,
                //        Cdep = f.Cdep,
                //        Carr = f.Carr,
                //        Ldep = f.Ldep,
                //        Larr = f.Larr,
                //        RouteNum = fr.RouteNum,

                //    }).ToList ();

                Dictionary<string, TripMultiVacData> allTripsMultiVacData = null;

                string currentBidName = WBidHelper.GenerateFileNameUsingCurrentBidDetails();


                string vACFileName = WBidHelper.GetAppDataPath() + "//" + currentBidName + ".VAC";
                //Cheks the VAC file exists
                bool vacFileExists = File.Exists(vACFileName);
                if (!vacFileExists)
                {
                    allTripsMultiVacData = new Dictionary<string, TripMultiVacData>();
                }
                else
                {

                    using (FileStream vacstream = File.OpenRead(vACFileName))
                    {

                        Dictionary<string, TripMultiVacData> objineinfo = new Dictionary<string, TripMultiVacData>();
                        allTripsMultiVacData = ProtoSerailizer.DeSerializeObject(vACFileName, objineinfo, vacstream);

                    }
                }


                VacationCorrectionParams vacationParams = new VacationCorrectionParams();
                vacationParams.FlightRouteDetails = GlobalSettings.FlightRouteDetails;

                vacationParams.CurrentBidDetails = GlobalSettings.CurrentBidDetails;
                vacationParams.Trips = GlobalSettings.Trip.ToDictionary(x => x.TripNum, x => x);
                vacationParams.Lines = GlobalSettings.Lines.ToDictionary(x => x.LineNum.ToString(), x => x);
                vacationParams.IsEOM = true;
                //  VacationData = new Dictionary<string, TripMultiVacData>();


                //Performing vacation correction algoritham
                VacationCorrectionBL vacationBL = new VacationCorrectionBL();
                //GlobalSettings.VacationData = vacationBL.PerformVacationCorrection(vacationParams);
                GlobalSettings.VacationData = vacationBL.PerformVacationCorrectionForEOMCP(vacationParams, allTripsMultiVacData);

                if (GlobalSettings.VacationData != null)
                {

                    string fileToSave = string.Empty;
                    fileToSave = WBidHelper.GenerateFileNameUsingCurrentBidDetails();


                    // save the VAC file to app data folder

                    var stream = File.Create(WBidHelper.GetAppDataPath() + "/" + fileToSave + ".VAC");
                    ProtoSerailizer.SerializeObject(WBidHelper.GetAppDataPath() + "/" + fileToSave + ".VAC", GlobalSettings.VacationData, stream);
                    stream.Dispose();
                    stream.Close();

                    //CaculateVacationDetails calVacationdetails = new CaculateVacationDetails();
                    //calVacationdetails.CalculateVacationdetailsFromVACfile(vacationParams.Lines, GlobalSettings.VacationData);

                    ////set the Vacpay,Vdrop,Vofont and VoBack columns in the line summary view 
                    //ManageVacationColumns managevacationcolumns = new ManageVacationColumns();
                    //managevacationcolumns.SetVacationColumns();

                    //LineInfo lineInfo = new LineInfo()
                    //{
                    //    LineVersion = GlobalSettings.LineVersion,
                    //    Lines = vacationParams.Lines

                    //};




                    //GlobalSettings.Lines = new System.Collections.ObjectModel.ObservableCollection<Line>(vacationParams.Lines.Select(x => x.Value));


                    //try
                    //{
                    //    var linestream = File.Create(WBidHelper.GetAppDataPath() + "/" + fileToSave + ".WBL");
                    //    ProtoSerailizer.SerializeObject(WBidHelper.GetAppDataPath() + "/" + fileToSave + ".WBL", lineInfo, linestream);
                    //    linestream.Dispose();
                    //    linestream.Close();
                    //}
                    //catch (Exception ex)
                    //{

                    //    CommonClass.AppDelegate.ErrorLog(ex);
                    //    CommonClass.AppDelegate.ShowErrorMessage(WBidErrorMessages.CommonError);
                    //}


                    //foreach (Line line in GlobalSettings.Lines)
                    //{
                    //    if (line.ConstraintPoints == null)
                    //        line.ConstraintPoints = new ConstraintPoints();
                    //    if (line.WeightPoints == null)
                    //        line.WeightPoints = new WeightPoints();
                    //}
                }
                else
                {
                    GlobalSettings.IsVacationCorrection = false;
                }
                //commented on 30-1-2024 because previous vacation got removed after applied the EOM
                //GlobalSettings.OrderedVacationDays = null;

            }
            catch (Exception ex)
            {
                GlobalSettings.OrderedVacationDays = null;

                CommonClass.AppDelegate.ErrorLog(ex);
                //CommonClass.AppDelegate.ShowErrorMessage (WBidErrorMessages.CommonError);
            }
        }

        public static DateTime GetnextSunday ()
        {
            DateTime date = GlobalSettings.CurrentBidDetails.BidPeriodEndDate;
            for (int count = 1; count <= 3; count++) {
                date = date.AddDays (1);
                if (date.DayOfWeek.ToString () == "Sunday")
                    break;
            }


            return date;
        }

        void handleEOMOptions(int option)
        {
            if (option == 0)
            {
                //btnEOM.Selected = true;
                btnEOM.State = NSCellStateValue.On;
                GlobalSettings.FAEOMStartDate = GlobalSettings.CurrentBidDetails.BidPeriodEndDate.AddDays(1);
                wBIdStateContent.FAEOMStartDate = GlobalSettings.FAEOMStartDate;

            }
            else if (option == 1)
            {
                //btnEOM.Selected = true;
                btnEOM.State = NSCellStateValue.On;
                GlobalSettings.FAEOMStartDate = GlobalSettings.CurrentBidDetails.BidPeriodEndDate.AddDays(2);
                wBIdStateContent.FAEOMStartDate = GlobalSettings.FAEOMStartDate;

            }
            else if (option == 2)
            {
                //btnEOM.Selected = true;
                btnEOM.State = NSCellStateValue.On;
                GlobalSettings.FAEOMStartDate = GlobalSettings.CurrentBidDetails.BidPeriodEndDate.AddDays(3);
                wBIdStateContent.FAEOMStartDate = GlobalSettings.FAEOMStartDate;

            }
            else
            {
                //btnEOM.Selected = false;
                btnEOM.State = NSCellStateValue.Off;
                GlobalSettings.UndoStack.RemoveAt(0);
                UpdateUndoRedoButtons();
                GlobalSettings.MenuBarButtonStatus.IsEOM = false;
                if (!(btnEOM.State == NSCellStateValue.On) && !(btnVacation.State == NSCellStateValue.On))
                {
                    btnDrop.Enabled = false;
                    btnDrop.State = NSCellStateValue.Off;
                }
                SetVacButtonStates();

            }
            if (GlobalSettings.CurrentBidDetails.Postion == "FA")
                CreateEOMforFA();
            else
                DownloadEOMData();
        }

        private void CreateEOMforFA ()
        {
            if (GlobalSettings.FAEOMStartDate != null && GlobalSettings.FAEOMStartDate != DateTime.MinValue) {
                btnDrop.Enabled = true;

                string overlayTxt = string.Empty;
                if (GlobalSettings.MenuBarButtonStatus.IsEOM)
                    overlayTxt = "Applying EOM";
                else
                {
                    if (!GlobalSettings.MenuBarButtonStatus.IsVacationCorrection)
                    {
                        //btnVacDrop.Selected = false;
                        btnDrop.State = NSCellStateValue.Off;
                        GlobalSettings.MenuBarButtonStatus.IsVacationDrop = false;
                    }
                    overlayTxt = "Removing EOM";
                }

                overlayPanel = new NSPanel ();
                overlayPanel.SetContentSize (new CGSize (400, 120));
                overlay = new OverlayViewController ();
                overlay.OverlayText = overlayTxt;
                overlayPanel.ContentView = overlay.View;
                NSApplication.SharedApplication.BeginSheet (overlayPanel, this.Window);
                InvokeOnMainThread (() => {
                    
                        CreateEOMVacforFA();
                    
                   

                    GenerateVacationDataView ();

                    InvokeOnMainThread (() => {
                        //loadSummaryListAndHeader();

                        NSApplication.SharedApplication.EndSheet (overlayPanel);
                        overlayPanel.OrderOut (this);

                        GlobalSettings.isModified = true;
                        ReloadAllContent ();
                        //CommonClass.lineVC.UpdateSaveButton();
                    });
                });



            }
        }

        private void CreateEOMVacforFA ()
        {
            VacationCorrectionParams vacationParams = new VacationCorrectionParams ();
            vacationParams.CurrentBidDetails = GlobalSettings.CurrentBidDetails;
            vacationParams.Trips = GlobalSettings.Trip.ToDictionary (x => x.TripNum, x => x);
            vacationParams.Lines = GlobalSettings.Lines.ToDictionary (x => x.LineNum.ToString (), x => x);
            Dictionary<string, TripMultiVacData> allTripsMultiVacData = null;

            string currentBidName = WBidHelper.GenerateFileNameUsingCurrentBidDetails ();


            string vACFileName = WBidHelper.GetAppDataPath () + "//" + currentBidName + ".VAC";
            //Cheks the VAC file exists
            bool vacFileExists = File.Exists (vACFileName);

            if (!vacFileExists) {
                allTripsMultiVacData = new Dictionary<string, TripMultiVacData> ();
            } else {

                using (FileStream vacstream = File.OpenRead (vACFileName)) {

                    Dictionary<string, TripMultiVacData> objineinfo = new Dictionary<string, TripMultiVacData> ();
                    allTripsMultiVacData = ProtoSerailizer.DeSerializeObject (vACFileName, objineinfo, vacstream);

                }
            }



            //Performing vacation correction algoritham
            
            if (GlobalSettings.IsObservedAlgm)
            {
                ObserveVacationCorrectionBL vacationBL = new ObserveVacationCorrectionBL();
                GlobalSettings.VacationData = vacationBL.CreateVACfileForEOMFA(vacationParams, allTripsMultiVacData);

            }
            else
            {
                VacationCorrectionBL vacationBL = new VacationCorrectionBL();
                GlobalSettings.VacationData = vacationBL.CreateVACfileForEOMFA(vacationParams, allTripsMultiVacData);
            }


            string fileToSave = string.Empty;
            fileToSave = WBidHelper.GenerateFileNameUsingCurrentBidDetails ();
            if (GlobalSettings.VacationData != null && GlobalSettings.VacationData.Count > 0) {




                // save the VAC file to app data folder

                var stream = File.Create (WBidHelper.GetAppDataPath () + "/" + fileToSave + ".VAC");
                ProtoSerailizer.SerializeObject (WBidHelper.GetAppDataPath () + "/" + fileToSave + ".VAC", GlobalSettings.VacationData, stream);
                stream.Dispose ();
                stream.Close ();

                if (GlobalSettings.IsObservedAlgm)
                {
                    ObserveCaculateVacationDetails observecalVacationdetails = new ObserveCaculateVacationDetails();
                    observecalVacationdetails.CalculateVacationdetailsFromVACfile(vacationParams.Lines, GlobalSettings.VacationData);
                }
                else
                {
                    CaculateVacationDetails calVacationdetails = new CaculateVacationDetails();
                    calVacationdetails.CalculateVacationdetailsFromVACfile(vacationParams.Lines, GlobalSettings.VacationData);
                }
               

                //set the Vacpay,Vdrop,Vofont and VoBack columns in the line summary view 
                ManageVacationColumns managevacationcolumns = new ManageVacationColumns ();
                managevacationcolumns.SetVacationColumns ();

                LineInfo lineInfo = new LineInfo () {
                    LineVersion = GlobalSettings.LineVersion,
                    Lines = vacationParams.Lines

                };




                GlobalSettings.Lines = new System.Collections.ObjectModel.ObservableCollection<Line> (vacationParams.Lines.Select (x => x.Value));


                try {
                    var linestream = File.Create (WBidHelper.GetAppDataPath () + "/" + fileToSave + ".WBL");
                    ProtoSerailizer.SerializeObject (WBidHelper.GetAppDataPath () + "/" + fileToSave + ".WBL", lineInfo, linestream);
                    linestream.Dispose ();
                    linestream.Close ();
                } catch (Exception ex) {

                    CommonClass.AppDelegate.ErrorLog (ex);
                    CommonClass.AppDelegate.ShowErrorMessage (WBidErrorMessages.CommonError);
                }


                foreach (Line line in GlobalSettings.Lines) {
                    if (line.ConstraintPoints == null)
                        line.ConstraintPoints = new ConstraintPoints ();
                    if (line.WeightPoints == null)
                        line.WeightPoints = new WeightPoints ();
                }

            }






        }

        void btnVacationDropClicked (object sender, EventArgs e)
        {

            WBidHelper.PushToUndoStack ();
            UpdateUndoRedoButtons ();
            GlobalSettings.MenuBarButtonStatus.IsVacationDrop = (btnDrop.State == NSCellStateValue.On);
            SetVacButtonStates ();
            SetPropertyNames ();
            WBidCollection.GenarateTempAbsenceList ();
            string overlayTxt = string.Empty;
            if (GlobalSettings.MenuBarButtonStatus.IsVacationDrop) {
                btnDrop.Title = "DRP";
                overlayTxt = "Applying Vacation Drop";
                VacationDropFunctinality (overlayTxt);
            } else 
            {

                var alert = new NSAlert ();
                alert.Window.Title = "WBidMax";
                alert.MessageText = "";
                alert.InformativeText = "Careful : You have turned OFF the DRP button. The lines will be adjusted after you close this dialog. They will be adjusted to show that you are flying the VDF(red) and VDB(red).\n\nIf you have “drop all” selected as your preference in CWA, then you should turn back on the DRP button to see the lines as they will be after the VDF and VDB are dropped.\n\nIf this does not make sense, please go to Help menu and select Help to read about Vacation Corrections for Pilots and Flight Attendants.";
                alert.AddButton ("OK");
                alert.Buttons [0].Activated += delegate {
                    
                    btnDrop.Title = "FLY";
                    TextColor(btnDrop, NSColor.White);
                    overlayTxt ="Removing Vacation Drop"; 
                    VacationDropFunctinality (overlayTxt);
                    alert.Window.Close ();
                    NSApplication.SharedApplication.StopModal ();
                    };
                alert.RunModal ();

            }



        }

        void VacationDropFunctinality (string overlayTxt)
        {
            overlayPanel = new NSPanel ();
            overlayPanel.SetContentSize (new CGSize (400, 120));
            overlay = new OverlayViewController ();
            overlay.OverlayText = overlayTxt;
            overlayPanel.ContentView = overlay.View;
            NSApplication.SharedApplication.BeginSheet (overlayPanel, this.Window);
            BeginInvokeOnMainThread (() =>  {
                try {
                    //                    PrepareModernBidLineView prepareModernBidLineView = new PrepareModernBidLineView ();
                    //                    RecalcalculateLineProperties RecalcalculateLineProperties = new RecalcalculateLineProperties ();
                    //                    prepareModernBidLineView.CalculatebidLinePropertiesforVacation ();
                    //                    RecalcalculateLineProperties.CalcalculateLineProperties ();
                    //SortLineList ();
                    StateManagement statemanagement = new StateManagement ();
                    WBidState wBidStateCont = GlobalSettings.WBidStateCollection.StateList.FirstOrDefault (x => x.StateName == GlobalSettings.WBidStateCollection.DefaultName);
                    statemanagement.RecalculateLineProperties (wBidStateCont);
                    statemanagement.ApplyCSW (wBidStateCont);
                }
                catch (Exception ex) {
                    InvokeOnMainThread (() =>  {
                        CommonClass.AppDelegate.ErrorLog (ex);
                        CommonClass.AppDelegate.ShowErrorMessage (WBidErrorMessages.CommonError);
                    });
                }
                InvokeOnMainThread (() =>  {
                    GlobalSettings.isModified = true;
                    //CommonClass.lineVC.UpdateSaveButton();
                    NSApplication.SharedApplication.EndSheet (overlayPanel);
                    overlayPanel.OrderOut (this);
                    ReloadAllContent ();
                });
            });
            var wBidStateContent = GlobalSettings.WBidStateCollection.StateList.FirstOrDefault (x => x.StateName == GlobalSettings.WBidStateCollection.DefaultName);
            wBidStateContent.MenuBarButtonState.IsVacationDrop = GlobalSettings.MenuBarButtonStatus.IsVacationDrop;
        }

        void btnVacationClicked (object sender, EventArgs e)
        {

            try {
                btnDrop.Title = "DRP";
                WBidHelper.PushToUndoStack ();
                UpdateUndoRedoButtons ();
                if (btnVacation.State == NSCellStateValue.On) {
                    //vacation button selected.
                   
                    GlobalSettings.MenuBarButtonStatus.IsVacationCorrection = true;
                    GlobalSettings.MenuBarButtonStatus.IsOverlap = false;
                    GlobalSettings.MenuBarButtonStatus.IsVacationDrop = true;
                    btnDrop.State = NSCellStateValue.On;
                } else {


                    //vacation button un selected.
                    GlobalSettings.MenuBarButtonStatus.IsVacationCorrection = false;
                    GlobalSettings.MenuBarButtonStatus.IsVacationDrop = true;
                    if (GlobalSettings.MenuBarButtonStatus.IsEOM == false) {

                        btnDrop.State = NSCellStateValue.Off;
                        GlobalSettings.MenuBarButtonStatus.IsVacationDrop = false;
                    }
                    TextColor(btnVacation, NSColor.Black);

                }


                SetVacButtonStates ();
                SetPropertyNames ();
                //GlobalSettings.MenuBarButtonStatus.IsVacationDrop = false;


                WBidCollection.GenarateTempAbsenceList ();
                string overlayTxt = string.Empty;
                if (GlobalSettings.MenuBarButtonStatus.IsVacationCorrection) {
                    overlayTxt = "Applying Vacation Correction";
                    //btnVacDrop.Enabled = true;

                } else {
                    overlayTxt = "Removing Vacation Correction";
                    //btnVacDrop.Enabled = false;

                }
                //foreach (var column in GlobalSettings.AdditionalColumns) {
                //    column.IsSelected = false;
                //}
                //var selectedColumns = GlobalSettings.AdditionalColumns.Where (x => GlobalSettings.WBidINIContent.DataColumns.Any (y => y.Id == x.Id)).ToList ();
                //foreach (var selectedColumn in selectedColumns) {
                //    selectedColumn.IsSelected = true;
                //}

                overlayPanel = new NSPanel ();
                overlayPanel.SetContentSize (new CGSize (400, 120));
                overlay = new OverlayViewController ();
                overlay.OverlayText = overlayTxt;
                overlayPanel.ContentView = overlay.View;
                NSApplication.SharedApplication.BeginSheet (overlayPanel, this.Window);


                BeginInvokeOnMainThread (() => {
                    try {

//                        PrepareModernBidLineView prepareModernBidLineView = new PrepareModernBidLineView ();
//                        RecalcalculateLineProperties RecalcalculateLineProperties = new RecalcalculateLineProperties ();
//                        prepareModernBidLineView.CalculatebidLinePropertiesforVacation ();
//                        RecalcalculateLineProperties.CalcalculateLineProperties ();
                        StateManagement statemanagement = new StateManagement ();
                        WBidState wBidStateCont = GlobalSettings.WBidStateCollection.StateList.FirstOrDefault (x => x.StateName == GlobalSettings.WBidStateCollection.DefaultName);
                        statemanagement.RecalculateLineProperties (wBidStateCont);
                        statemanagement.ApplyCSW (wBidStateCont);
                        //SortLineList ();
                    } catch (Exception ex) {
                        InvokeOnMainThread (() => {
                            CommonClass.AppDelegate.ErrorLog (ex);
                            CommonClass.AppDelegate.ShowErrorMessage (WBidErrorMessages.CommonError);
//                            throw ex;
                        });
                    }

                    InvokeOnMainThread (() => {

                        //loadSummaryListAndHeader ();

                        GlobalSettings.isModified = true;
                        //CommonClass.lineVC.UpdateSaveButton();
                        NSApplication.SharedApplication.EndSheet (overlayPanel);
                        overlayPanel.OrderOut (this);
                        ReloadAllContent ();
                    });
                });


                var wBidStateContent = GlobalSettings.WBidStateCollection.StateList.FirstOrDefault (x => x.StateName == GlobalSettings.WBidStateCollection.DefaultName);
                wBidStateContent.MenuBarButtonState.IsVacationCorrection = GlobalSettings.MenuBarButtonStatus.IsVacationCorrection;


            } catch (Exception ex) {

                CommonClass.AppDelegate.ErrorLog (ex);
                CommonClass.AppDelegate.ShowErrorMessage (WBidErrorMessages.CommonError);
            }
        }

        public void SetVacButtonStates ()
        {
            if (GlobalSettings.IsOverlapCorrection) {
                btnOverlap.Enabled = (!GlobalSettings.MenuBarButtonStatus.IsVacationCorrection && !GlobalSettings.MenuBarButtonStatus.IsEOM && !GlobalSettings.MenuBarButtonStatus.IsMIL);

            } else {
                btnOverlap.Enabled = false;
                GlobalSettings.MenuBarButtonStatus.IsOverlap = false;
            }

            if (GlobalSettings.IsVacationCorrection || GlobalSettings.IsFVVacation) {
                btnVacation.Enabled = (!GlobalSettings.MenuBarButtonStatus.IsOverlap && !GlobalSettings.MenuBarButtonStatus.IsMIL);

            } else {
                btnVacation.Enabled = false;

            }


            btnEOM.Enabled = (!GlobalSettings.MenuBarButtonStatus.IsOverlap && !GlobalSettings.MenuBarButtonStatus.IsMIL);

            btnDrop.Enabled = ((GlobalSettings.MenuBarButtonStatus.IsVacationCorrection && GlobalSettings.OrderedVacationDays != null && GlobalSettings.OrderedVacationDays.Count > 0) || GlobalSettings.MenuBarButtonStatus.IsEOM);
            //btnDrop.Enabled = (GlobalSettings.MenuBarButtonStatus.IsVacationCorrection || GlobalSettings.MenuBarButtonStatus.IsEOM);

            if (!GlobalSettings.MenuBarButtonStatus.IsVacationCorrection && !GlobalSettings.MenuBarButtonStatus.IsEOM) {
                GlobalSettings.MenuBarButtonStatus.IsVacationDrop = false;
            }

            
            btnMIL.Enabled = (!GlobalSettings.MenuBarButtonStatus.IsOverlap && !GlobalSettings.MenuBarButtonStatus.IsVacationCorrection && !GlobalSettings.MenuBarButtonStatus.IsEOM);

            btnEOM.State = (GlobalSettings.MenuBarButtonStatus.IsEOM) ? NSCellStateValue.On : NSCellStateValue.Off;
            btnVacation.State = (GlobalSettings.MenuBarButtonStatus.IsVacationCorrection) ? NSCellStateValue.On : NSCellStateValue.Off;
            btnDrop.State = (GlobalSettings.MenuBarButtonStatus.IsVacationDrop) ? NSCellStateValue.On : NSCellStateValue.Off;
            btnOverlap.State = (GlobalSettings.MenuBarButtonStatus.IsOverlap) ? NSCellStateValue.On : NSCellStateValue.Off;
            btnMIL.State = (GlobalSettings.MenuBarButtonStatus.IsMIL) ? NSCellStateValue.On : NSCellStateValue.Off;

            SetPropertyNames ();

            btnOverlap.BezelStyle = (btnOverlap.Enabled) ? NSBezelStyle.TexturedRounded : NSBezelStyle.SmallSquare;
            btnVacation.BezelStyle = (btnVacation.Enabled) ? NSBezelStyle.TexturedRounded : NSBezelStyle.SmallSquare;
            btnDrop.BezelStyle = (btnDrop.Enabled) ? NSBezelStyle.TexturedRounded : NSBezelStyle.SmallSquare;
            btnEOM.BezelStyle = (btnEOM.Enabled) ? NSBezelStyle.TexturedRounded : NSBezelStyle.SmallSquare;
            btnMIL.BezelStyle = (btnMIL.Enabled) ? NSBezelStyle.TexturedRounded : NSBezelStyle.SmallSquare;

            if (GlobalSettings.WBidStateCollection.DataSource == "HistoricalData") {
                
            }
            if (GlobalSettings.WBidStateCollection != null)
            {
                if(GlobalSettings.WBidStateCollection.DataSource == "HistoricalData")
                {
                    btnEOM.Enabled = false;
                    btnDrop.Enabled = false;
                    btnVacation.Enabled = false;
                    btnOverlap.Enabled = false;
                    btnMIL.Enabled = false;
                }
                WBidState WBidStateContent = GlobalSettings.WBidStateCollection.StateList.FirstOrDefault (x => x.StateName == GlobalSettings.WBidStateCollection.DefaultName);
                if (wBIdStateContent.IsMissingTripFailed) {
                    btnMIL.Enabled = false;
                }
            }
            //if ((GlobalSettings.MenuBarButtonStatus.IsVacationCorrection || GlobalSettings.MenuBarButtonStatus.IsEOM) && GlobalSettings.MenuBarButtonStatus.IsVacationDrop == false)
            //{
            //    TextColor(btnDrop, NSColor.White);
            //}
            //else
            //{
            //    TextColor(btnDrop, NSColor.Black);
            //}
            if (btnVacation.State == NSCellStateValue.On)
            {
                btnVacation.Image = NSImage.ImageNamed("btnOrangeBg.png");
            }
            else
            {
                btnVacation.Image = NSImage.ImageNamed("btnLightGreenBg.png");
            }
            if (btnDrop.State == NSCellStateValue.On)
            {
                btnDrop.Image = NSImage.ImageNamed("btnOrangeBg.png");
            }
            else
            {
                if (GlobalSettings.MenuBarButtonStatus.IsVacationCorrection == false && GlobalSettings.MenuBarButtonStatus.IsEOM == false)
                    btnDrop.Image = NSImage.ImageNamed("btnLightGreenBg.png");
                else if (GlobalSettings.MenuBarButtonStatus.IsVacationCorrection || GlobalSettings.MenuBarButtonStatus.IsEOM)
                    btnDrop.Image = NSImage.ImageNamed("btnRedBg.png");
               

                //if (GlobalSettings.MenuBarButtonStatus.IsVacationCorrection && GlobalSettings.MenuBarButtonStatus.IsEOM)
                //    //    btnDrop.Image = NSImage.ImageNamed("btnRedBg.png");
                //    //else
                //    if (btnDrop.StringValue == "FLY")
                //        btnDrop.Image = NSImage.ImageNamed("btnRedBg.png");
                //    else
                //        btnDrop.Image = NSImage.ImageNamed("btnLightGreenBg.png");
            }
            if (btnEOM.State == NSCellStateValue.On)
            {
                btnEOM.Image = NSImage.ImageNamed("btnOrangeBg.png");
            }
            else
            {
                btnEOM.Image = NSImage.ImageNamed("btnLightGreenBg.png");
            }

        }

        private void applyOverLapCorrection ()
        {
            string overlayTxt = string.Empty;
            ReCalculateLinePropertiesForOverlapCorrection reCalculateLinePropertiesForOverlapCorrection = new ReCalculateLinePropertiesForOverlapCorrection ();
            overlayTxt = "Applying Overlap Correction";

            SetVacButtonStates ();


            BeginInvokeOnMainThread (() => {
                overlayPanel = new NSPanel ();
                overlayPanel.SetContentSize (new CGSize (400, 120));
                overlay = new OverlayViewController ();
                overlay.OverlayText = overlayTxt;
                overlayPanel.ContentView = overlay.View;
                NSApplication.SharedApplication.BeginSheet (overlayPanel, this.Window);
                try {
                    reCalculateLinePropertiesForOverlapCorrection.ReCalculateLinePropertiesOnOverlapCorrection (GlobalSettings.Lines.ToList (), true);
                    SortLineList ();
                } catch (Exception ex) {
                    InvokeOnMainThread (() => {
                        throw ex;
                    });
                }

                InvokeOnMainThread (() => {
                    NSApplication.SharedApplication.EndSheet (overlayPanel);
                    overlayPanel.OrderOut (this);
                    ReloadAllContent ();
                });
            });
        }

        void btnOverLapClicked (object sender, EventArgs e)
        {
            try {

                WBidHelper.PushToUndoStack ();
                UpdateUndoRedoButtons ();
//                ReCalculateLinePropertiesForOverlapCorrection reCalculateLinePropertiesForOverlapCorrection = new ReCalculateLinePropertiesForOverlapCorrection ();
                string overlayTxt = string.Empty;
                
                if (btnOverlap.State == NSCellStateValue.On) {
                
                    GlobalSettings.MenuBarButtonStatus.IsVacationCorrection = false;
                    GlobalSettings.MenuBarButtonStatus.IsVacationDrop = false;
                    GlobalSettings.MenuBarButtonStatus.IsEOM = false;
                    GlobalSettings.MenuBarButtonStatus.IsOverlap = true;
                    overlayTxt = "Applying Overlap Correction";
                
                    SetVacButtonStates ();
                
                
                    overlayPanel = new NSPanel ();
                    overlayPanel.SetContentSize (new CGSize (400, 120));
                    overlay = new OverlayViewController ();
                    overlay.OverlayText = overlayTxt;
                    overlayPanel.ContentView = overlay.View;
                    NSApplication.SharedApplication.BeginSheet (overlayPanel, this.Window);
                
                    //LoadingOverlay overlay = new LoadingOverlay(this.View.Frame, overlayTxt);
                    //this.View.Add(overlay);
                    BeginInvokeOnMainThread (() => {
                        try {
//                            reCalculateLinePropertiesForOverlapCorrection.ReCalculateLinePropertiesOnOverlapCorrection (GlobalSettings.Lines.ToList (), true);
//                            SortLineList ();
                            StateManagement statemanagement = new StateManagement ();
                            WBidState wBidStateCont = GlobalSettings.WBidStateCollection.StateList.FirstOrDefault (x => x.StateName == GlobalSettings.WBidStateCollection.DefaultName);
                            statemanagement.RecalculateLineProperties (wBidStateCont);
                            statemanagement.ApplyCSW (wBidStateCont);
                        } catch (Exception ex) {
                            InvokeOnMainThread (() => {
                                throw ex;
                            });
                        }
                
                        InvokeOnMainThread (() => {
                            NSApplication.SharedApplication.EndSheet (overlayPanel);
                            overlayPanel.OrderOut (this);
                            ReloadAllContent ();
                            //                                CommonClass.SummaryController.ReloadContent();
                
                            GlobalSettings.isModified = true;
                            //CommonClass.lineVC.UpdateSaveButton();
                        });
                    });
                } else {
                    GlobalSettings.MenuBarButtonStatus.IsVacationCorrection = false;
                    GlobalSettings.MenuBarButtonStatus.IsVacationDrop = false;
                    GlobalSettings.MenuBarButtonStatus.IsEOM = false;
                    GlobalSettings.MenuBarButtonStatus.IsOverlap = false;
                    overlayTxt = "Removing Overlap Correction";
                
                    SetVacButtonStates ();
                
                    overlayPanel = new NSPanel ();
                    overlayPanel.SetContentSize (new CGSize (400, 120));
                    overlay = new OverlayViewController ();
                    overlay.OverlayText = overlayTxt;
                    overlayPanel.ContentView = overlay.View;
                    NSApplication.SharedApplication.BeginSheet (overlayPanel, this.Window);
                
                
                
                    BeginInvokeOnMainThread (() => {
//                        reCalculateLinePropertiesForOverlapCorrection.ReCalculateLinePropertiesOnOverlapCorrection (GlobalSettings.Lines.ToList (), false);
                        StateManagement statemanagement = new StateManagement ();
                        WBidState wBidStateCont = GlobalSettings.WBidStateCollection.StateList.FirstOrDefault (x => x.StateName == GlobalSettings.WBidStateCollection.DefaultName);
                        statemanagement.RecalculateLineProperties (wBidStateCont);
                        statemanagement.ApplyCSW (wBidStateCont);
                        InvokeOnMainThread (() => {
                
                            NSApplication.SharedApplication.EndSheet (overlayPanel);
                            overlayPanel.OrderOut (this);
                            ReloadAllContent ();
                            //                                CommonClass.SummaryController.ReloadContent();
                
                            GlobalSettings.isModified = true;
                            //CommonClass.lineVC.UpdateSaveButton();
                        });
                    });
                
                }
                var wBidStateContent = GlobalSettings.WBidStateCollection.StateList.FirstOrDefault (x => x.StateName == GlobalSettings.WBidStateCollection.DefaultName);
                wBidStateContent.MenuBarButtonState.IsOverlap = GlobalSettings.MenuBarButtonStatus.IsOverlap;
            } catch (Exception ex) {
                CommonClass.AppDelegate.ErrorLog (ex);
                CommonClass.AppDelegate.ShowErrorMessage (WBidErrorMessages.CommonError);
            }
        }
        //        void btnCSWClicked (object sender, EventArgs e)
        //        {
        //            if (cswWC == null)
        //                cswWC = new CSWWindowController ();
        //            CommonClass.CSWController = cswWC;
        ////                this.Window.SetFrameOrigin (NSScreen.MainScreen.VisibleFrame.Location);
        ////                this.Window.SetContentSize (new System.Drawing.SizeF (NSScreen.MainScreen.VisibleFrame.Width - cswWC.Window.Frame.Size.Width, this.Window.Frame.Size.Height));
        ////                cswWC.Window.SetFrameOrigin(new System.Drawing.PointF(NSScreen.MainScreen.VisibleFrame.Width - cswWC.Window.Frame.Size.Width,cswWC.Window.Frame.Location.Y));
        //            this.Window.AddChildWindow (cswWC.Window, NSWindowOrderingMode.Above);
        //            cswWC.Window.MakeKeyWindow ();
        //        }

        void btnCSWClicked (object sender, EventArgs e)
        {
            ShowCSW ();
        }

        
        void btnBAClicked (object sender, EventArgs e)
        {
            ShowBA ();
        }


        void HandleSecretDownload(object sender, EventArgs e)
        {
            ShowSecretDownload();
        }

        void btnSaveClicked (object sender, EventArgs e)
        {
            try {
                StateManagement stateManagement = new StateManagement ();
                stateManagement.UpdateWBidStateContent ();
                //CompareState compareState = new CompareState ();
                //string fileName = WBidHelper.GenerateFileNameUsingCurrentBidDetails ();
                //var WbidCollection = XmlHelper.ReadStateFile(WBidHelper.GetAppDataPath() + "/" + fileName + ".WBS");
                //var wBIdStateContent = WbidCollection.StateList.FirstOrDefault(x => x.StateName == GlobalSettings.WBidStateCollection.DefaultName);
                //bool isNochange = compareState.CompareStateChange(wBIdStateContent, GlobalSettings.WBidStateCollection.StateList.FirstOrDefault(x => x.StateName == GlobalSettings.WBidStateCollection.DefaultName));
                if (GlobalSettings.isModified) {
                
                    GlobalSettings.WBidStateCollection.IsModified = true;
//                     StateManagement stateManagement = new StateManagement();
//                     stateManagement.UpdateWBidStateContent();
                    WBidHelper.SaveStateFile (WBidHelper.WBidStateFilePath);
                    if (timer != null) {
                        timer.Stop ();
                        timer.Start ();
                    }
                    //    save the state of the INI File
                    WBidHelper.SaveINIFile (GlobalSettings.WBidINIContent, WBidHelper.GetWBidINIFilePath ());
                    UpdateSaveButton (false);
                }
            } catch (Exception ex) {
                CommonClass.AppDelegate.ErrorLog (ex);
                CommonClass.AppDelegate.ShowErrorMessage (WBidErrorMessages.CommonError);
            }

        }

        void btnHomeClicked (object sender, EventArgs e)
        {
            try {
                SetScreenSize();
                if (GlobalSettings.isModified) {
                    var alert = new NSAlert ();
                    alert.Window.Title = "WBidMax";
                    alert.MessageText = "Save your Changes?";
                    //alert.InformativeText = "There are no Latest News available..!";

                    int saveindex = 0;
                    if (GlobalSettings.WBidINIContent.User.SmartSynch) {
                        alert.AddButton ("Save & Synch");
                        saveindex = 1;
                        alert.Buttons [0].Activated += delegate {
                            StateManagement stateManagement = new StateManagement ();
                            stateManagement.UpdateWBidStateContent ();
                            GlobalSettings.WBidStateCollection.IsModified = true;
                            WBidHelper.SaveStateFile (WBidHelper.WBidStateFilePath);
                            alert.Window.Close ();
                            NSApplication.SharedApplication.StopModal ();
                            isNeedToClose = 1;
                            //SynchState ();
                            Synch();
                        };
                    }
                    alert.AddButton ("Save & Exit");
                    alert.AddButton ("Exit");
                    alert.AddButton ("Cancel");
                    alert.Buttons [saveindex].Activated += delegate {
                        // save and exit
                        StateManagement stateManagement = new StateManagement ();
                        stateManagement.UpdateWBidStateContent ();
                        if (GlobalSettings.isModified) {
                            GlobalSettings.WBidStateCollection.IsModified = true;
                            WBidHelper.SaveStateFile (WBidHelper.WBidStateFilePath);
                        }
                        //GoToHomeWindow ();
                        alert.Window.Close ();
                        NSApplication.SharedApplication.StopModal ();
                        isNeedToClose = 1;
                        CheckSmartSync ();
                        //Timer disposing - exititng to home screen
                        if (timer != null) {
                            timer.Stop ();
                            timer.Close();
                            timer.Dispose();

                        }
                    };
                    alert.Buttons [saveindex + 1].Activated += delegate {
                        //Timer disposing - exititng to home screen
                        if (timer != null) {
                            
                            timer.Stop ();
                            timer.Close();
                            timer.Dispose();

                        }
                        GoToHomeWindow ();
                        alert.Window.Close ();
                        NSApplication.SharedApplication.StopModal ();
                    };
                    alert.RunModal ();
                } else {
                    if (GlobalSettings.WBidStateCollection.IsModified ||(GlobalSettings.QuickSets!=null && GlobalSettings.QuickSets.IsModified)) {
                        isNeedToClose = 1;
                        CheckSmartSync ();
                    } else
                        GoToHomeWindow ();
                }

                //save the state of the INI File
                WBidHelper.SaveINIFile (GlobalSettings.WBidINIContent, WBidHelper.GetWBidINIFilePath ());
            } catch (Exception ex) {
                CommonClass.AppDelegate.ErrorLog (ex);
                CommonClass.AppDelegate.ShowErrorMessage (WBidErrorMessages.CommonError);
            }
        }

        void GoToHomeWindow ()
        {
            if (cswWC != null)
                cswWC.Window.Close();

            if (synchView != null)
                synchView.Window.Close();

            if (baWC != null)
                baWC.Window.Close();
            
            if (qsWC != null)
                qsWC.Window.Close();

            if ( pairing!= null)
                pairing.Window.Close();
            if (reloadNotification != null)
            {
                    NSNotificationCenter.DefaultCenter.RemoveObserver (reloadNotification);
            }

            if (confNotif != null)
            {
                NSNotificationCenter.DefaultCenter.RemoveObserver(confNotif);
            }


            CommonClass.columnID = 0;
            CommonClass.isHomeWindow = true;
            CommonClass.HomeController.Window.MakeKeyAndOrderFront (this);
            this.Window.Close ();
            this.Window.OrderOut (this);
            CommonClass.AppDelegate.ReloadMenu ();
            if (this.Window.ChildWindows != null) {
                foreach (var item in this.Window.ChildWindows) {
                    item.Close ();
                }
            }
            if (CommonClass.Panel != null) {
                NSApplication.SharedApplication.EndSheet (CommonClass.Panel);
                CommonClass.Panel.OrderOut (this);
            }
        }

        public void ShowClendarView ()
        {
            if (calendarWC == null)
                calendarWC = new CalendarWindowController ();
            CommonClass.CalendarController = calendarWC;
            this.Window.AddChildWindow (calendarWC.Window, NSWindowOrderingMode.Above);
            calendarWC.Window.MakeKeyAndOrderFront (this);
            calendarWC.LoadContent ();
        }

        public void MoveLineUp ()
        {
            summaryVC.MoveLineUp ();
        }

        public void MoveLineDown ()
        { 
            summaryVC.MoveLineDown ();
        }

        void btnRemTopLockClicked (object sender, EventArgs e)
        {
            StateManagement stateManagement = new StateManagement ();
            stateManagement.UpdateWBidStateContent ();
            WBidHelper.PushToUndoStack ();

            RemoveTopLock ();
        }

        void btnRemBottomLockClicked (object sender, EventArgs e)
        {
            try {
                StateManagement stateManagement = new StateManagement ();
                stateManagement.UpdateWBidStateContent ();
                WBidHelper.PushToUndoStack ();

                btnBottomLock.Enabled = true;
                LineOperations.RemoveAllBottomLock ();
                ReloadAllContent ();
                if (calendarWC != null)
                    calendarWC.Window.Close ();
            } catch (Exception ex) {
                CommonClass.AppDelegate.ErrorLog (ex);
                CommonClass.AppDelegate.ShowErrorMessage (WBidErrorMessages.CommonError);
            }
        }


        public void RemoveTopLock ()
        {
            //modified on july 2017 according to the requirmment "when the top lock is remove, SORT should revert to MANUAL and none of the lines should be resorted
            wBIdStateContent.SortDetails.SortColumn = "Manual";
            if (CommonClass.SortController != null)
            {
                CommonClass.SortController.setValuesToFixedSorts();
                CommonClass.SortController.setViews();
            }
            btnTopLock.Enabled = true;
            LineOperations.RemoveAllTopLock ();
            ReloadAllContent ();
            if (calendarWC != null)
                calendarWC.Window.Close ();


        }

        public void RemoveBottomLock ()
        {
            //modified on july 2017 according to the requirmment "when the top lock is remove, SORT should revert to MANUAL and none of the lines should be resorted
            wBIdStateContent.SortDetails.SortColumn = "Manual";
            if (CommonClass.SortController != null)
            {
                CommonClass.SortController.setValuesToFixedSorts();
                CommonClass.SortController.setViews();
            }
            btnBottomLock.Enabled = true;
            LineOperations.RemoveAllBottomLock ();
            ReloadAllContent ();
            if (calendarWC != null)
                calendarWC.Window.Close ();
        }

        public void LockBtnEnableDisable ()
        {
            int none = 0;
            int top = 0;
            int bot = 0;
            foreach (var item in CommonClass.selectedRows) {
                if (!GlobalSettings.Lines.FirstOrDefault (x => x.LineNum == item).TopLock && !GlobalSettings.Lines.FirstOrDefault (x => x.LineNum == item).BotLock)
                    none++;
                if (GlobalSettings.Lines.FirstOrDefault (x => x.LineNum == item).TopLock)
                    top++;
                if (GlobalSettings.Lines.FirstOrDefault (x => x.LineNum == item).BotLock)
                    bot++;
            }
            if (none > 0) {
                btnTopLock.Enabled = true;
                btnBottomLock.Enabled = true;
            } else if (top > 0) {
                btnTopLock.Enabled = false;
                btnBottomLock.Enabled = true;
            } else if (bot > 0) {
                btnTopLock.Enabled = true;
                btnBottomLock.Enabled = false;
            } else {
                btnTopLock.Enabled = false;
                btnBottomLock.Enabled = false;
            }
        }

        public void RemoveBtnEnableDisable ()
        {
            btnRemTopLock.Enabled = (GlobalSettings.Lines.Count (x => x.TopLock) > 0);
            btnRemBottomLock.Enabled = (GlobalSettings.Lines.Count (x => x.BotLock) > 0);
            CommonClass.AppDelegate.menuRemoveToplock.Enabled = btnRemTopLock.Enabled;
            CommonClass.AppDelegate.menuRemoveBottomLock.Enabled = btnRemBottomLock.Enabled;


        }

        public void ResetAll ()
        {

            StateManagement stateManagement = new StateManagement ();
            stateManagement.UpdateWBidStateContent ();
            WBidHelper.PushToUndoStack ();

            var alert = new NSAlert ();
            alert.AlertStyle = NSAlertStyle.Critical;
            alert.MessageText = "Reset All";
            alert.InformativeText = "Do you want to Reset All?";
            alert.AddButton ("YES");
            alert.AddButton ("NO");
            alert.Buttons [0].Activated += (object sender, EventArgs e) => {
                ClearTopAndBottomLocks ();
                ClearConstraints ();
                ClearWeights ();
                ClearSort ();
                //clear group number
                GlobalSettings.Lines.ToList().ForEach(x => { x.BAGroup = string.Empty; x.IsGrpColorOn = 0; });

                //    WBidHelper.PushToUndoStack ();


                if (wBIdStateContent.BidAuto != null)
                {
                    wBIdStateContent.BidAuto.BAGroup = new List<BidAutoGroup>();

                    //Reset Bid Automator settings.
                    wBIdStateContent.BidAuto.BAFilter = new List<BidAutoItem>();

                    wBIdStateContent.BidAuto.BASort = new SortDetails();
                }
                CommonClass.columnID = 0;
                CommonClass.MainController.ReloadAllContent ();
                if(CommonClass.BAController!=null)
                {
                CommonClass.BAController.ReloadAllContent ();
                }
                if (CommonClass.CSWController != null) {
                    //CommonClass.CSWController.ShowWindow(CommonClass.MainController);
                    CommonClass.CSWController.CloseCSW ();
                    CommonClass.CSWController = null;
                    this.PerformSelector (new ObjCRuntime.Selector ("ShowCSWView"), null, 0.5);

                    //CommonClass.CSWController.ChangeView();
                }
                if(CommonClass.synchVController != null)
                {
                    CommonClass.synchVController.CloseSynch();
                    CommonClass.synchVController = null;

                }
                if (qsWC != null)
                {
                    //CommonClass.CSWController.ShowWindow(CommonClass.MainController);

                    qsWC.Window.Close();
                    //CommonClass.CSWController.ChangeView();
                }

                if (baWC != null)
                {
                    //CommonClass.CSWController.ShowWindow(CommonClass.MainController);

                    baWC.Window.Close();
                    //CommonClass.CSWController.ChangeView();
                }
                if (pairing != null)
                    pairing.Window.Close();

                alert.Window.Close ();
                NSApplication.SharedApplication.StopModal ();
            };
            alert.RunModal ();
        }


        [Export("ShowCSWView")]
        void ShowCSWView()
        {
            CommonClass.MainController.ShowCSWFromReset();
        }
        public void ClearConstraintsAndweights ()
        {
            var alert = new NSAlert ();
            alert.AlertStyle = NSAlertStyle.Critical;
            alert.MessageText = "Reset Constraints and Weights";
            alert.InformativeText = "Do you want to Clear All Constraints and Weights";
            alert.AddButton ("YES");
            alert.AddButton ("NO");
            alert.Buttons [0].Activated += (object sender, EventArgs e) => {
                ClearConstraints ();
                ClearWeights ();
                CommonClass.MainController.ReloadAllContent ();
                if (CommonClass.CSWController != null) {
                    CommonClass.CSWController.ReloadAllContent ();
                }
                alert.Window.Close ();
                NSApplication.SharedApplication.StopModal ();
            };
            alert.RunModal ();

        }

        private void ClearTopAndBottomLocks ()
        {
            GlobalSettings.Lines.ToList ().ForEach (x => {
                x.TopLock = false;
                x.BotLock = false;
            });
        }

        public void ClearConstraints ()
        {
            try {
                List<int> lstOff = new List<int> () { };
                
                List<int> lstWork = new List<int> () { };
                //var requiredLines = GlobalSettings.Lines.Where(x => !x.BlankLine);
                foreach (Line line in GlobalSettings.Lines) {
                    line.ConstraintPoints.Reset ();
                    line.Constrained = false;
                
                }
                
                WBidState currentState = GlobalSettings.WBidStateCollection.StateList.FirstOrDefault (x => x.StateName == GlobalSettings.WBidStateCollection.DefaultName);
                
                CxWtState states = currentState.CxWtState;
                
                currentState.Constraints.Hard = false;
                currentState.Constraints.Ready = false;
                currentState.Constraints.Reserve = false;
                currentState.Constraints.Blank = false;
                currentState.Constraints.International = false;
                currentState.Constraints.NonConus = false;
                currentState.Constraints.ETOPS = false;
                
                currentState.CxWtState.AMPMMIX.AM = false;
                currentState.CxWtState.AMPMMIX.PM = false;
                currentState.CxWtState.AMPMMIX.MIX = false;
                
                currentState.CxWtState.FaPosition.A = false;
                currentState.CxWtState.FaPosition.B = false;
                currentState.CxWtState.FaPosition.C = false;
                currentState.CxWtState.FaPosition.D = false;
                
                currentState.CxWtState.TripLength.Turns = false;
                currentState.CxWtState.TripLength.Twoday = false;
                currentState.CxWtState.TripLength.ThreeDay = false;
                currentState.CxWtState.TripLength.FourDay = false;
                
                currentState.CxWtState.DaysOfWeek.MON = false;
                currentState.CxWtState.DaysOfWeek.TUE = false;
                currentState.CxWtState.DaysOfWeek.WED = false;
                currentState.CxWtState.DaysOfWeek.THU = false;
                currentState.CxWtState.DaysOfWeek.FRI = false;
                currentState.CxWtState.DaysOfWeek.SAT = false;
                currentState.CxWtState.DaysOfWeek.SUN = false;
                
                
                
                states.ACChg.Cx = false;
                //states.AMPM.Cx = false;
                states.BDO.Cx = false;
                states.CL.Cx = false;
                states.CLAuto.Cx = false;
                states.DHD.Cx = false;
                states.DHDFoL.Cx = false;
                states.DOW.Cx = false;
                states.DP.Cx = false;
                states.EQUIP.Cx = false;
                states.FLTMIN.Cx = false;
                states.GRD.Cx = false;
                states.InterConus.Cx = false;
                states.LEGS.Cx = false;
                states.LegsPerPairing.Cx = false;
                states.MP.Cx = false;
                states.No3on3off.Cx = false;
                states.NODO.Cx = false;
                states.NOL.Cx = false;
                states.PerDiem.Cx = false;
                states.Rest.Cx = false;
                states.RON.Cx = false;
                states.SDO.Cx = false;
                states.SDOW.Cx = false;
                states.TL.Cx = false;
                states.WB.Cx = false;
                states.WtPDOFS.Cx = false;
                states.LrgBlkDaysOff.Cx = false;
                states.Position.Cx = false;
                states.WorkDay.Cx = false;
                states.BulkOC.Cx = false;
                states.CitiesLegs.Cx=false;
                states.Commute.Cx=false;
                states.StartDay.Cx = false;
                states.ReportRelease.Cx = false;
                states.MixedHardReserveTrip.Cx = false;
                // states.InterCon.Cx = false;
                //Update the state object
                
                Commutability commutecx;
                if (currentState.CxWtState.Commute.Wt || currentState.SortDetails.BlokSort.Contains("30")|| currentState.SortDetails.BlokSort.Contains("31") || currentState.SortDetails.BlokSort.Contains("32"))
                    commutecx = new Commutability
                {
                    City = currentState.Constraints.Commute.City,
                    BaseTime = currentState.Constraints.Commute.BaseTime,
                    ConnectTime = currentState.Constraints.Commute.ConnectTime,
                    CheckInTime = currentState.Constraints.Commute.CheckInTime,
                    SecondcellValue = (int)CommutabilitySecondCell.NoMiddle,
                    ThirdcellValue = (int)CommutabilityThirdCell.Overall,
                    Type = (int)ConstraintType.MoreThan,
                    Value = 100
                };
                else
                    commutecx = new Commutability
                {
                    BaseTime = 10,
                    ConnectTime = 30,
                    CheckInTime = 60,
                    SecondcellValue = (int)CommutabilitySecondCell.NoMiddle,
                    ThirdcellValue = (int)CommutabilityThirdCell.Overall,
                    Type = (int)ConstraintType.MoreThan,
                    Value = 100
                };
                FtCommutableLine clauto;
                if (currentState.CxWtState.CLAuto.Wt || currentState.SortDetails.BlokSort.Contains("33") || currentState.SortDetails.BlokSort.Contains("34") || currentState.SortDetails.BlokSort.Contains("35"))
                {
                    clauto = new FtCommutableLine()
                    {
                        City = currentState.Constraints.CLAuto.City,
                        ToHome = currentState.Constraints.CLAuto.ToHome,
                        ToWork = currentState.Constraints.CLAuto.ToWork,
                        NoNights = currentState.Constraints.CLAuto.NoNights,
                        BaseTime = currentState.Constraints.CLAuto.BaseTime,
                        ConnectTime = currentState.Constraints.CLAuto.ConnectTime,
                        CheckInTime = currentState.Constraints.CLAuto.CheckInTime,
                        IsNonStopOnly = currentState.Constraints.CLAuto.IsNonStopOnly
                    };
                }
                else
                {
                    clauto = new FtCommutableLine()
                    {
                        
                        ToHome = true,
                        ToWork = false,
                        NoNights = false,
                        BaseTime = 10,
                        ConnectTime = 30,
                        CheckInTime = 120
                    };
                }

                Constraints constraint = new Constraints() {
                    Hard = false,
                    Ready = false,
                    Reserve = false,
                    International = false,
                    NonConus = false,
                    ETOPS = false,
                    // AM_PM = new AMPMConstriants{AM=false,PM=false,MIX=false},
                    LrgBlkDayOff = new Cx2Parameter { Type = (int)ConstraintType.LessThan, Value = 10 },
                    AircraftChanges = new Cx2Parameter { Type = (int)ConstraintType.MoreThan, Value = 4 },
                    BlockOfDaysOff = new Cx2Parameter { Type = (int)ConstraintType.LessThan, Value = 18 },
                    DeadHeads = new Cx4Parameters {
                        SecondcellValue = "1",
                        ThirdcellValue = ((int)DeadheadType.First).ToString(),
                        Type = (int)ConstraintType.LessThan,
                        Value = 1,
                        LstParameter = new List<Cx4Parameter>()
                    },
                    CL = new CxCommutableLine() {
                        AnyNight = true,
                        RunBoth = false,
                        CommuteToHome = true,
                        CommuteToWork = true,
                        MondayThu = new Times() { Checkin = 0, BackToBase = 0 },
                        MondayThuDefault = new Times() { Checkin = 0, BackToBase = 0 },
                        Friday = new Times() { Checkin = 0, BackToBase = 0 },
                        FridayDefault = new Times() { Checkin = 0, BackToBase = 0 },
                        Saturday = new Times() { Checkin = 0, BackToBase = 0 },
                        SaturdayDefault = new Times() { Checkin = 0, BackToBase = 0 },
                        Sunday = new Times() { Checkin = 0, BackToBase = 0 },
                        SundayDefault = new Times() { Checkin = 0, BackToBase = 0 },
                        TimesList = new List<Times>()

                    },
                    Commute = commutecx,
                    CLAuto = clauto,
                    DeadHeadFoL = new Cx3Parameters {
                        ThirdcellValue = ((int)DeadheadType.First).ToString(),
                        Type = (int)ConstraintType.LessThan,
                        Value = 1,
                        lstParameters = new List<Cx3Parameter>()
                    },
                    DutyPeriod = new Cx2Parameter { Type = (int)ConstraintType.MoreThan, Value = 600 },

                    EQUIP = new Cx3Parameters {
                        ThirdcellValue = "700",
                        Type = (int)ConstraintType.MoreThan,
                        Value = 0,
                        lstParameters = new List<Cx3Parameter>()
                    },
                    FlightMin = new Cx2Parameter { Type = (int)ConstraintType.MoreThan, Value = 7200 },
                    GroundTime = new Cx3Parameter { Type = (int)ConstraintType.MoreThan, Value = 1, ThirdcellValue = "30" },
                    InterConus = new Cx2Parameters() {
                        Type = (int)CityType.International,
                        Value = 1,
                        lstParameters = new List<Cx2Parameter>()
                    },
                    LegsPerDutyPeriod = new Cx2Parameter { Type = (int)ConstraintType.MoreThan, Value = 4 },
                    LegsPerPairing = new Cx2Parameter { Type = (int)ConstraintType.MoreThan, Value = 18 },
                    NumberOfDaysOff = new Cx2Parameter { Type = (int)ConstraintType.LessThan, Value = 18 },

                    OverNightCities = new Cx3Parameters {
                        ThirdcellValue = "6",
                        Type = (int)ConstraintType.LessThan,
                        Value = 1,
                        lstParameters = new List<Cx3Parameter>()
                    },
                    CitiesLegs = new Cx3Parameters {
                        ThirdcellValue = "1",
                        Type = (int)ConstraintType.LessThan,
                        Value = 1,
                        lstParameters = new List<Cx3Parameter>()
                    },
                    BulkOvernightCity = new BulkOvernightCityCx { OverNightNo = new List<int>(), OverNightYes = new List<int>() },
                    PDOFS = new Cx4Parameters { SecondcellValue = "300", ThirdcellValue = "400", Type = (int)ConstraintType.atafter, Value = 915, LstParameter = new List<Cx4Parameter>() },
                    Position = new Cx3Parameters {
                        Type = (int)ConstraintType.LessThan,
                        Value = 1,
                        lstParameters = new List<Cx3Parameter>()
                    },
                    StartDayOftheWeek = new Cx3Parameters {
                        SecondcellValue = "1",
                        ThirdcellValue = "6",
                        Type = (int)ConstraintType.MoreThan,
                        Value = 3,
                        lstParameters = new List<Cx3Parameter>()
                    },
                    Rest = new Cx3Parameters {
                        ThirdcellValue = "1",
                        Type = (int)ConstraintType.LessThan,
                        Value = 8,
                        lstParameters = new List<Cx3Parameter>()
                    },
                    PerDiem = new Cx2Parameter { Type = (int)ConstraintType.MoreThan, Value = 18000 },
                    TripLength = new Cx3Parameters {
                        ThirdcellValue = "4",
                        Type = (int)ConstraintType.MoreThan,
                        Value = 1,
                        lstParameters = new List<Cx3Parameter>()
                    },
                    WorkBlockLength = new Cx3Parameters {
                        ThirdcellValue = "4",
                        Type = (int)ConstraintType.LessThan,
                        Value = 2,
                        lstParameters = new List<Cx3Parameter>()
                    },
                    MinimumPay = new Cx2Parameter { Type = (int)ConstraintType.MoreThan, Value = 90 },
                    No3On3Off = new Cx2Parameter { Type = (int)ThreeOnThreeOff.ThreeOnThreeOff, Value = 10 },
                    WorkDay = new Cx2Parameter { Type = (int)ConstraintType.LessThan, Value = 11 },
                    StartDay = new Cx3Parameters { ThirdcellValue = "1", Type = 1, Value = 1 },
                    ReportRelease = new ReportReleases { AllDays = true, First = false, Last = false, NoMid = false, Report = 0, Release = 0 },
                    DaysOfMonth = new DaysOfMonthCx() { OFFDays = lstOff, WorkDays = lstWork },
                    MixedHardReserveTrip = false,
                    No1Or2Off = new Cx2Parameter { Type = (int)OneOr2Off.NoOneOr2Off, Value = 10 }




                };
                if (constraint.Commute.City != null)
                {
                    constraint.DailyCommuteTimesCmmutability = currentState.Constraints.DailyCommuteTimesCmmutability;
                }
                currentState.Constraints = constraint;
            } catch (Exception ex) {
                CommonClass.AppDelegate.ErrorLog (ex);
                CommonClass.AppDelegate.ShowErrorMessage (WBidErrorMessages.CommonError);
            }
        }

        public void ClearWeights()
        {
            try
            {
                ///Reset all weight values to zero.
                GlobalSettings.Lines.ToList().ForEach(x =>
                {
                    x.WeightPoints.Reset();
                    x.TotWeight = 0.0m;
                });
                //ApplyLineOrderBasedOnWeight();

                WBidState currentState = GlobalSettings.WBidStateCollection.StateList.FirstOrDefault(x => x.StateName == GlobalSettings.WBidStateCollection.DefaultName);

                CxWtState states = currentState.CxWtState;


                states.ACChg.Wt = false;
                states.AMPM.Wt = false;
                states.BDO.Wt = false;
                states.CL.Wt = false;
                states.DHD.Wt = false;
                states.DOW.Wt = false;
                states.DHDFoL.Wt = false;
                states.DP.Wt = false;
                states.EQUIP.Wt = false;
                states.ETOPS.Wt = false;
                states.ETOPSRes.Wt = false;
                states.FLTMIN.Wt = false;
                states.GRD.Wt = false;
                states.InterConus.Wt = false;
                states.LrgBlkDaysOff.Wt = false;
                states.LEGS.Wt = false;
                states.LegsPerPairing.Wt = false;
                states.MP.Wt = false;
                states.No3on3off.Wt = false;
                states.NODO.Wt = false;
                states.NOL.Wt = false;
                states.PerDiem.Wt = false;
                states.PDAfter.Wt = false;
                states.PDBefore.Wt = false;
                states.Position.Wt = false;
                states.Rest.Wt = false;
                states.RON.Wt = false;
                states.SDO.Wt = false;
                states.SDOW.Wt = false;
                states.TL.Wt = false;
                states.WB.Wt = false;
                states.WorkDay.Wt = false;
                states.NormalizeDays.Wt = false;
                states.BulkOC.Wt = false;
                states.CitiesLegs.Wt = false;
                states.Commute.Wt = false;
                states.CLAuto.Wt = false;
                //Weights
                //----------------------------------------

                var oldclDeafult = currentState.Weights.CL.DefaultTimes;
                var weight = new Weights
                {
                    AirCraftChanges = new Wt3Parameter { SecondlValue = 1, ThrirdCellValue = 1, Weight = 0 },
                    //AMM,PM, Night
                    AM_PM = new Wt2Parameters { Type = 1, Weight = 0, lstParameters = new List<Wt2Parameter>() },
                    LrgBlkDayOff = new Wt2Parameter { Weight = 0 },

                    BDO = new Wt3Parameters
                    {
                        SecondlValue = 1,
                        ThrirdCellValue = 1,
                        Weight = 0
                            ,
                        lstParameters = new List<Wt3Parameter>()
                    },
                    DHD = new Wt3Parameters
                    {
                        SecondlValue = 1,
                        ThrirdCellValue = 1,
                        Weight = 0,
                        lstParameters = new List<Wt3Parameter>()
                    },
                    //Commutable Line
                    CL = new WtCommutableLine()
                    {
                        TimesList = new List<Times>() {
                            new Times (){ Checkin = 0, BackToBase = 0 },
                            new Times (){ Checkin = 0, BackToBase = 0 },
                            new Times (){ Checkin = 0, BackToBase = 0 },
                            new Times (){ Checkin = 0, BackToBase = 0 },
                        },
                        DefaultTimes = new List<Times>() {
                            new Times (){ Checkin = 0, BackToBase = 0 },
                            new Times (){ Checkin = 0, BackToBase = 0 },
                            new Times (){ Checkin = 0, BackToBase = 0 },
                            new Times (){ Checkin = 0, BackToBase = 0 },
                        },
                        BothEnds = 0,
                        InDomicile = 0,
                        Type = 1
                        //1.  All 2. 

                    },

                    Commute = new Commutability { SecondcellValue = (int)CommutabilitySecondCell.NoMiddle, ThirdcellValue = (int)CommutabilityThirdCell.Overall, Type = (int)ConstraintType.MoreThan, Value = 100 },

                    SDO = new DaysOfMonthWt()
                    {
                        isWork = false,
                        Weights = new List<Wt>()
                    },

                    DOW = new WtDaysOfWeek()
                    {
                        lstWeight = new List<Wt>() { new Wt() { Key = 0, Value = 0 } },
                        IsOff = true

                    },
                    DP = new Wt3Parameters()
                    {
                        SecondlValue = 1,
                        ThrirdCellValue = 300,
                        Weight = 0
                            ,
                        lstParameters = new List<Wt3Parameter>()
                    },

                    EQUIP = new Wt3Parameters
                    {
                        SecondlValue = 300,
                        ThrirdCellValue = 1,
                        Weight = 0,
                        lstParameters = new List<Wt3Parameter>()
                    },

                    ETOPS = new Wt1Parameters
                    {
                        Weight = 0,
                        lstParameters = new List<Wt1Parameter>()
                    },

                    ETOPSRes = new Wt1Parameters
                    {
                        
                        Weight = 0,
                        lstParameters = new List<Wt1Parameter>()
                    },

                    FLTMIN = new Wt3Parameters
                    {
                        SecondlValue = 0,
                        ThrirdCellValue = 20,
                        Weight = 0,
                        lstParameters = new List<Wt3Parameter>()
                    },

                    GRD = new Wt3Parameters()
                    {
                        SecondlValue = 0,
                        ThrirdCellValue = 1,
                        Weight = 0,
                        lstParameters = new List<Wt3Parameter>()
                    },
                    InterConus = new Wt2Parameters()
                    {
                        Type = -1,
                        Weight = 0,
                        lstParameters = new List<Wt2Parameter>()
                    },
                    LEGS = new Wt3Parameters
                    {
                        SecondlValue = 1,
                        ThrirdCellValue = 1,
                        Weight = 0,
                        lstParameters = new List<Wt3Parameter>()
                    },
                    WtLegsPerPairing = new Wt3Parameters()
                    {
                        SecondlValue = 1,
                        ThrirdCellValue = 1,
                        Weight = 0,
                        lstParameters = new List<Wt3Parameter>()
                    },

                    NODO = new Wt2Parameters
                    {
                        Type = 9,
                        Weight = 0,
                        lstParameters = new List<Wt2Parameter>()
                    },
                    RON = new Wt2Parameters
                    {
                        Type = 1,
                        Weight = 0,
                        lstParameters = new List<Wt2Parameter>()
                    },
                    CitiesLegs = new Wt2Parameters
                    {
                        Type = 1,
                        Weight = 0,
                        lstParameters = new List<Wt2Parameter>()
                    },
                    SDOW = new Wt2Parameters
                    {
                        Type = 1,
                        Weight = 0,
                        lstParameters = new List<Wt2Parameter>()
                    },
                    WtRest = new Wt4Parameters
                    {
                        FirstValue = 1,
                        SecondlValue = 480,
                        ThrirdCellValue = 1,
                        Weight = 0,
                        lstParameters = new List<Wt4Parameter>()
                    },
                    PerDiem = new Wt2Parameter
                    {
                        Type = 100,
                        Weight = 0

                    },
                    TL = new Wt2Parameters
                    {
                        Type = 1,
                        Weight = 0,
                        lstParameters = new List<Wt2Parameter>()
                    },
                    WB = new Wt2Parameters
                    {
                        Type = 1,
                        Weight = 0,
                        lstParameters = new List<Wt2Parameter>()
                    },
                    POS = new Wt2Parameters
                    {
                        Type = 1,
                        Weight = 0,
                        lstParameters = new List<Wt2Parameter>()
                    },
                    DHDFoL = new Wt2Parameters
                    {
                        Type = 1,
                        Weight = 0,
                        lstParameters = new List<Wt2Parameter>()
                    },
                    WorkDays = new Wt3Parameters
                    {
                        SecondlValue = 1,
                        ThrirdCellValue = 1,
                        Weight = 0,
                        lstParameters = new List<Wt3Parameter>()
                    },

                    PDAfter = new Wt4Parameters
                    {
                        FirstValue = 300,
                        SecondlValue = 180,
                        ThrirdCellValue = 400,
                        Weight = 0,
                        lstParameters = new List<Wt4Parameter>()
                    },
                    PDBefore = new Wt4Parameters
                    {
                        FirstValue = 300,
                        SecondlValue = 180,
                        ThrirdCellValue = 400,
                        Weight = 0,
                        lstParameters = new List<Wt4Parameter>()
                    },
                    //CLAuto=new FtCommutableLine()
                    //{
                    //    ToHome = true,
                    //    ToWork = false,
                    //    NoNights=false
                    //}
                    NormalizeDaysOff = new Wt2Parameter { Type = 1, Weight = 0 },


                    OvernightCitybulk = new List<Wt2Parameter>()
                    //if (currentState.CxWtState.Commute.Cx == false && !(currentState.SortDetails.BlokSort.Contains("30") || currentState.SortDetails.BlokSort.Contains("31") || currentState.SortDetails.BlokSort.Contains("32")))
                    //        {
                    //            currentState.Constraints.Commute.City = string.Empty;

                    //            foreach (var line in Lines)
                    //            {
                    //                line.CommutableBacks = 0;
                    //                line.commutableFronts = 0;
                    //                line.CommutabilityFront = 0;
                    //                line.CommutabilityBack = 0;
                    //                line.CommutabilityOverall = 0;
                    //            }
                    //        }
                    //        currentState.Weights = weight;
                };
                currentState.Weights = weight;

            }
            catch (Exception ex)
            {
                CommonClass.AppDelegate.ErrorLog(ex);
                CommonClass.AppDelegate.ShowErrorMessage(WBidErrorMessages.CommonError);
            }
        }
        

        private void ClearSort ()
        {
            //SortCalculation sort = new SortCalculation();
            //sort.SortLines("Line");
            var state = GlobalSettings.WBidStateCollection.StateList.FirstOrDefault (x => x.StateName == GlobalSettings.WBidStateCollection.DefaultName);
            state.ForceLine.IsBlankLinetoBottom = false;
            state.ForceLine.IsReverseLinetoBottom = false;
            state.SortDetails.SortColumn = "Line";
            state.SortDetails.SortDirection = string.Empty;
        }


        void HandleViewSelect (object sender, EventArgs e)
        {
            try {
                ChangeView ();
                CommonClass.ViewChanged = true;
                ReloadAllContent ();
                CommonClass.ViewChanged = false;
            } catch (Exception ex) {
                CommonClass.AppDelegate.ErrorLog (ex);
                CommonClass.AppDelegate.ShowErrorMessage (WBidErrorMessages.CommonError);
            }
        }

        void btnBottomLockClicked (object sender, EventArgs e)
        {
            try {
                StateManagement stateManagement = new StateManagement ();
                stateManagement.UpdateWBidStateContent ();
                WBidHelper.PushToUndoStack ();

                LineOperations.TrashLines (CommonClass.selectedRows);
                ReloadAllContent ();
                CommonClass.selectedRows.Clear ();
                CommonClass.columnID = 0;
                if (calendarWC != null)
                    calendarWC.Window.Close ();
                if (summaryVC != null && sgViewSelect.SelectedSegment == 0) {
                    MoveLineUp ();
                    MoveLineDown ();
                }
            } catch (Exception ex) {
                CommonClass.AppDelegate.ErrorLog (ex);
                CommonClass.AppDelegate.ShowErrorMessage (WBidErrorMessages.CommonError);
            }

        }

        void btnTopLockClicked (object sender, EventArgs e)
        {
            try {
                StateManagement stateManagement = new StateManagement ();
                stateManagement.UpdateWBidStateContent ();
                WBidHelper.PushToUndoStack ();

                LineOperations.PromoteLines (CommonClass.selectedRows);
                ReloadAllContent ();
                CommonClass.selectedRows.Clear ();
                CommonClass.columnID = 0;
                if (calendarWC != null)
                    calendarWC.Window.Close ();
                if (summaryVC != null && sgViewSelect.SelectedSegment == 0)
                    MoveLineDown ();
            } catch (Exception ex) {
                CommonClass.AppDelegate.ErrorLog (ex);
                CommonClass.AppDelegate.ShowErrorMessage (WBidErrorMessages.CommonError);
            }

        }

        public void TopLock ()
        {
            try {
                StateManagement stateManagement = new StateManagement ();
                stateManagement.UpdateWBidStateContent ();
                WBidHelper.PushToUndoStack ();

                LineOperations.PromoteLines (CommonClass.selectedRows);
                //summaryVC.ReloadContent ();
                ReloadAllContent ();
            } catch (Exception ex) {
                CommonClass.AppDelegate.ErrorLog (ex);
                CommonClass.AppDelegate.ShowErrorMessage (WBidErrorMessages.CommonError);
            }
        }

        public void BottomLock ()
        {
            try {
                StateManagement stateManagement = new StateManagement ();
                stateManagement.UpdateWBidStateContent ();
                WBidHelper.PushToUndoStack ();

                LineOperations.TrashLines (CommonClass.selectedRows);
                //summaryVC.ReloadContent ();
                ReloadAllContent ();
            } catch (Exception ex) {
                CommonClass.AppDelegate.ErrorLog (ex);
                CommonClass.AppDelegate.ShowErrorMessage (WBidErrorMessages.CommonError);
            }
        }

        void ChangeView ()
          {
            try {
                
                foreach (var vw in vwMain.Subviews) {

//                    if (vw.GetType().Name == "SummaryViewController")
                        
                        
                    vw.RemoveFromSuperview ();

                }
                summaryVC = null;
                bidlineVC = null;
                modernVC= null;
                CommonClass.SummaryController=null;
                CommonClass.BidLineController=null;
                CommonClass.ModernController=null;

                if (sgViewSelect.SelectedSegment == 0) {
                    GlobalSettings.WBidINIContent.ViewType = 1;
                    if (summaryVC == null) {
                        summaryVC = new SummaryViewController ();
                    }
                    //this.Window.ContentView = summaryVC.View;
                    summaryVC.View.Frame = vwMain.Bounds;
                    vwMain.AddSubview (summaryVC.View);
                    CommonClass.SummaryController = summaryVC;
                } else if (sgViewSelect.SelectedSegment == 1) {
                    GlobalSettings.WBidINIContent.ViewType = 2;
                    if (bidlineVC == null) {
                        bidlineVC = new BidLineViewController ();
                    }
                    //this.Window.ContentView = bidlineVC.View;
                    bidlineVC.View.Frame = vwMain.Bounds;
                    vwMain.AddSubview (bidlineVC.View);
                    CommonClass.BidLineController = bidlineVC;
                } else if (sgViewSelect.SelectedSegment == 2) {
                    GlobalSettings.WBidINIContent.ViewType = 3;
                    if (modernVC == null) {
                        modernVC = new ModernViewController ();
                    }
                    //this.Window.ContentView = modernVC.View;
                    modernVC.View.Frame = vwMain.Bounds;
                    vwMain.AddSubview (modernVC.View);
                    CommonClass.ModernController = modernVC;
                }
                if (calendarWC != null)
                    calendarWC.Window.Close ();
                if (bidlineVC != null && bidlineVC.TripWC != null)
                    bidlineVC.TripWC.Window.Close ();
                if (modernVC != null && modernVC.TripWC != null)
                    modernVC.TripWC.Window.Close ();

            } catch (Exception ex) {
                CommonClass.AppDelegate.ErrorLog (ex);
                CommonClass.AppDelegate.ShowErrorMessage (WBidErrorMessages.CommonError);
            }

        }

        public void ReloadAllContent ()
        {
            try {
                if (CommonClass.SummaryController != null)
                {    CommonClass.SummaryController.LoadContent ();
                    //CommonClass.SummaryController.PromoteLine();
                }
                if (CommonClass.BidLineController != null)
                    CommonClass.BidLineController.ReloadContent ();
                if (CommonClass.ModernController != null)
                    CommonClass.ModernController.ReloadContent ();
                LockBtnEnableDisable ();
                RemoveBtnEnableDisable ();

                if (!CommonClass.ViewChanged)
                    UpdateSaveButton (true);
                UpdateUndoRedoButtons ();

                HandleBlueShadowButton();

            } catch (Exception ex) {
                CommonClass.AppDelegate.ErrorLog (ex);
                CommonClass.AppDelegate.ShowErrorMessage (WBidErrorMessages.CommonError);
            }
        }

        public void HandleBlueShadowButton()
        {
            var isBlueLineExists = GlobalSettings.Lines.FirstOrDefault(x => x.ManualScroll == 1 || x.ManualScroll == 2 || x.ManualScroll == 3);
            //if (GlobalSettings.WBidINIContent.User.IsModernViewShade)
            if (isBlueLineExists != null && GlobalSettings.WBidINIContent.User.IsModernViewShade == true)
            {

                //btnBlueShade.Image = new NSImage("blueBorderSelected.png");
                btnBlueShade.Image = NSImage.ImageNamed("blueBorderSelected.png");
            }
            else
            {
                //btnBlueShade.Image = new NSImage("blueBorder.png");
                btnBlueShade.Image = NSImage.ImageNamed("blueBorder.png");
            }
        }
        private static void SortLineList ()
        {
            SortCalculation sort = new SortCalculation ();
            WBidState wBidStateContent = GlobalSettings.WBidStateCollection.StateList.FirstOrDefault (x => x.StateName == GlobalSettings.WBidStateCollection.DefaultName);
            if (wBidStateContent.SortDetails != null && wBidStateContent.SortDetails.SortColumn != null && wBidStateContent.SortDetails.SortColumn != string.Empty) {
                sort.SortLines (wBidStateContent.SortDetails.SortColumn);
            }
        }

    }
}


