using CoreGraphics;
using Foundation;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using UIKit;
using Xamarin.Forms;

namespace Xamarin.Forms.Platform.Mac
{
  internal class ContextActionsCell : UITableViewCell, INativeElementView
  {
    private readonly List<MenuItem> menuItems = new List<MenuItem>();
    private readonly List<UIButton> buttons = new List<UIButton>();
    public const string Key = "ContextActionsCell";
    private Cell cell;
    private UIScrollView scroller;
    private UITableView tableView;
    private UIButton moreButton;
    private static readonly UIImage destructiveBackground;
    private static readonly UIImage normalBackground;

    Element INativeElementView.Element
    {
      get
      {
        INativeElementView nativeElementView = this.ContentCell as INativeElementView;
        if (nativeElementView == null)
          throw new InvalidOperationException("Implement IBoxedCell on cell renderer: " + this.ContentCell.GetType().AssemblyQualifiedName);
        return nativeElementView.Element;
      }
    }

    public UITableViewCell ContentCell { get; private set; }

    public bool IsOpen
    {
      get
      {
        return this.ScrollDelegate.IsOpen;
      }
    }

    private ContextScrollViewDelegate ScrollDelegate
    {
      get
      {
        return (ContextScrollViewDelegate) this.scroller.Delegate;
      }
    }

    static ContextActionsCell()
    {
      CGRect cgRect = new CGRect((nfloat) 0, (nfloat) 0, (nfloat) 1, (nfloat) 1);
      UIGraphics.BeginImageContext(cgRect.Size);
      CGContext currentContext = UIGraphics.GetCurrentContext();
      nfloat red = (nfloat) 1;
      nfloat green = (nfloat) 0;
      nfloat blue = (nfloat) 0;
      nfloat alpha = (nfloat) 1;
      currentContext.SetFillColor(red, green, blue, alpha);
      CGRect rect1 = cgRect;
      currentContext.FillRect(rect1);
      ContextActionsCell.destructiveBackground = UIGraphics.GetImageFromCurrentImageContext();
      CGColor color = ColorExtensions.ToCGColor(ColorExtensions.ToColor(UIColor.LightGray));
      currentContext.SetFillColor(color);
      CGRect rect2 = cgRect;
      currentContext.FillRect(rect2);
      ContextActionsCell.normalBackground = UIGraphics.GetImageFromCurrentImageContext();
      currentContext.Dispose();
    }

    public ContextActionsCell()
      : base(UITableViewCellStyle.Default, "ContextActionsCell")
    {
    }

    public void PrepareForDeselect()
    {
      this.ScrollDelegate.PrepareForDeselect(this.scroller);
    }

    public void Update(UITableView tableView, Cell cell, UITableViewCell nativeCell)
    {
      ListView listView = cell.Parent as ListView;
      bool flag = listView != null && listView.CachingStrategy == ListViewCachingStrategy.RecycleElement;
      if (this.cell != cell & flag)
      {
        if (this.cell != null)
          ((INotifyCollectionChanged) this.cell.get_ContextActions()).CollectionChanged -= new NotifyCollectionChangedEventHandler(this.OnContextItemsChanged);
        ((INotifyCollectionChanged) cell.get_ContextActions()).CollectionChanged += new NotifyCollectionChangedEventHandler(this.OnContextItemsChanged);
      }
      CGRect frame = this.Frame;
      nfloat height = frame.Height;
      frame = tableView.Frame;
      nfloat width1 = frame.Width;
      nativeCell.Frame = new CGRect((nfloat) 0, (nfloat) 0, width1, height);
      // ISSUE: reference to a compiler-generated method
      nativeCell.SetNeedsLayout();
      PropertyChangedEventHandler changedEventHandler = new PropertyChangedEventHandler(this.OnMenuItemPropertyChanged);
      this.tableView = tableView;
      this.SetupSelection(tableView);
      if (this.cell != null)
      {
        if (!flag)
          this.cell.remove_PropertyChanged(new PropertyChangedEventHandler(this.OnCellPropertyChanged));
        if (this.menuItems.Count > 0)
        {
          if (!flag)
            ((INotifyCollectionChanged) this.cell.get_ContextActions()).CollectionChanged -= new NotifyCollectionChangedEventHandler(this.OnContextItemsChanged);
          foreach (BindableObject bindableObject in this.menuItems)
            bindableObject.remove_PropertyChanged(changedEventHandler);
        }
        this.menuItems.Clear();
      }
      this.menuItems.AddRange((IEnumerable<MenuItem>) cell.get_ContextActions());
      this.cell = cell;
      if (!flag)
      {
        cell.add_PropertyChanged(new PropertyChangedEventHandler(this.OnCellPropertyChanged));
        ((INotifyCollectionChanged) this.cell.get_ContextActions()).CollectionChanged += new NotifyCollectionChangedEventHandler(this.OnContextItemsChanged);
      }
      bool isOpen = false;
      if (this.scroller == null)
      {
        this.scroller = new UIScrollView(new CGRect((nfloat) 0, (nfloat) 0, width1, height));
        this.scroller.ScrollsToTop = false;
        this.scroller.ShowsHorizontalScrollIndicator = false;
        if (Forms.IsiOS8OrNewer)
          this.scroller.PreservesSuperviewLayoutMargins = true;
        // ISSUE: reference to a compiler-generated method
        this.ContentView.AddSubview((UIView) this.scroller);
      }
      else
      {
        this.scroller.Frame = new CGRect((nfloat) 0, (nfloat) 0, width1, height);
        isOpen = this.ScrollDelegate.IsOpen;
        for (int index = 0; index < this.buttons.Count; ++index)
        {
          UIButton uiButton = this.buttons[index];
          // ISSUE: reference to a compiler-generated method
          uiButton.RemoveFromSuperview();
          uiButton.Dispose();
        }
        this.buttons.Clear();
        this.ScrollDelegate.Unhook(this.scroller);
        this.ScrollDelegate.Dispose();
      }
      if (this.ContentCell != nativeCell)
      {
        if (this.ContentCell != null)
        {
          // ISSUE: reference to a compiler-generated method
          this.ContentCell.RemoveFromSuperview();
          this.ContentCell = (UITableViewCell) null;
        }
        this.ContentCell = nativeCell;
        CellTableViewCell cellTableViewCell = this.ContentCell as CellTableViewCell;
        if ((cellTableViewCell != null ? cellTableViewCell.Cell : (Cell) null) is ImageCell)
        {
          nfloat left = (nfloat) 57;
          nfloat right = (nfloat) 0;
          if (UIDevice.CurrentDevice.UserInterfaceIdiom == UIUserInterfaceIdiom.Pad)
          {
            left = (nfloat) 89;
            right = left / (nfloat) 2;
          }
          this.SeparatorInset = new UIEdgeInsets((nfloat) 0, left, (nfloat) 0, right);
        }
        // ISSUE: reference to a compiler-generated method
        this.scroller.AddSubview((UIView) nativeCell);
      }
      this.SetupButtons(width1, height);
      UIView uiView = (UIView) null;
      nfloat width2 = width1;
      for (int index = this.buttons.Count - 1; index >= 0; --index)
      {
        UIButton uiButton = this.buttons[index];
        nfloat nfloat = width2;
        frame = uiButton.Frame;
        nfloat width3 = frame.Width;
        width2 = nfloat + width3;
        if (UIDevice.CurrentDevice.CheckSystemVersion(8, 0))
        {
          // ISSUE: reference to a compiler-generated method
          this.scroller.AddSubview((UIView) uiButton);
        }
        else
        {
          if (uiView == null)
          {
            frame = uiButton.Frame;
            uiView = (UIView) new iOS7ButtonContainer(frame.Width);
            // ISSUE: reference to a compiler-generated method
            this.scroller.InsertSubview(uiView, (nint) 0);
          }
          // ISSUE: reference to a compiler-generated method
          uiView.AddSubview((UIView) uiButton);
        }
      }
      this.scroller.Delegate = (IUIScrollViewDelegate) new ContextScrollViewDelegate(uiView, this.buttons, isOpen);
      this.scroller.ContentSize = new CGSize(width2, height);
      if (isOpen)
      {
        // ISSUE: reference to a compiler-generated method
        this.scroller.SetContentOffset(new CGPoint(this.ScrollDelegate.ButtonsWidth, (nfloat) 0), false);
      }
      else
      {
        // ISSUE: reference to a compiler-generated method
        this.scroller.SetContentOffset(new CGPoint((nfloat) 0, (nfloat) 0), false);
      }
    }

    private void SetupSelection(UITableView table)
    {
      for (int index = 0; index < table.GestureRecognizers.Length; ++index)
      {
        if (table.GestureRecognizers[index] is ContextActionsCell.SelectGestureRecognizer)
          return;
      }
      // ISSUE: reference to a compiler-generated method
      this.tableView.AddGestureRecognizer((UIGestureRecognizer) new ContextActionsCell.SelectGestureRecognizer());
    }

    public override void LayoutSubviews()
    {
      // ISSUE: reference to a compiler-generated method
      base.LayoutSubviews();
      if (this.scroller == null || this.scroller != null && this.scroller.Frame == this.Bounds)
        return;
      this.Update(this.tableView, this.cell, this.ContentCell);
      this.scroller.Frame = this.Bounds;
      this.ContentCell.Frame = this.Bounds;
    }

    public override CGSize SizeThatFits(CGSize size)
    {
      // ISSUE: reference to a compiler-generated method
      return this.ContentCell.SizeThatFits(size);
    }

    protected override void Dispose(bool disposing)
    {
      if (disposing)
      {
        if (this.scroller != null)
        {
          this.scroller.Dispose();
          this.scroller = (UIScrollView) null;
        }
        this.tableView = (UITableView) null;
        if (this.moreButton != null)
        {
          this.moreButton.Dispose();
          this.moreButton = (UIButton) null;
        }
        for (int index = 0; index < this.buttons.Count; ++index)
          this.buttons[index].Dispose();
        this.buttons.Clear();
        this.menuItems.Clear();
        if (this.cell != null)
        {
          if (this.cell.HasContextActions)
            ((INotifyCollectionChanged) this.cell.get_ContextActions()).CollectionChanged -= new NotifyCollectionChangedEventHandler(this.OnContextItemsChanged);
          this.cell = (Cell) null;
        }
      }
      // ISSUE: reference to a compiler-generated method
      base.Dispose(disposing);
    }

    private UIView SetupButtons(nfloat width, nfloat height)
    {
      MenuItem menuItem1 = (MenuItem) null;
      nfloat largestButtonWidth = (nfloat) 0;
      nfloat acceptableTotalWidth = width * (nfloat) 0.8f;
      for (int index = 0; index < this.cell.get_ContextActions().Count; ++index)
      {
        MenuItem menuItem2 = this.cell.get_ContextActions()[index];
        if (this.buttons.Count == 3)
        {
          if (menuItem1 == null)
          {
            if (menuItem2.IsDestructive)
              this.buttons.RemoveAt(this.buttons.Count - 1);
            else
              continue;
          }
          else
            break;
        }
        if (menuItem2.IsDestructive)
          menuItem1 = menuItem2;
        UIButton button = this.GetButton(menuItem2);
        button.Tag = (nint) index;
        // ISSUE: reference to a compiler-generated method
        nfloat nfloat = button.TitleLabel.SizeThatFits(new CGSize(width, height)).Width + (nfloat) 30;
        if (nfloat > largestButtonWidth)
          largestButtonWidth = nfloat;
        if (menuItem1 == menuItem2)
          this.buttons.Insert(0, button);
        else
          this.buttons.Add(button);
      }
      bool needMoreButton = this.cell.get_ContextActions().Count > this.buttons.Count;
      if (this.cell.get_ContextActions().Count > 2)
        this.CullButtons(acceptableTotalWidth, ref needMoreButton, ref largestButtonWidth);
      bool flag = false;
      if (needMoreButton)
      {
        if (largestButtonWidth * (nfloat) 2 > acceptableTotalWidth)
        {
          largestButtonWidth = acceptableTotalWidth / (nfloat) 2;
          flag = true;
        }
        UIButton uiButton = new UIButton(new CGRect((nfloat) 0, (nfloat) 0, largestButtonWidth, height));
        // ISSUE: reference to a compiler-generated method
        uiButton.SetBackgroundImage(ContextActionsCell.normalBackground, UIControlState.Normal);
        uiButton.TitleEdgeInsets = new UIEdgeInsets((nfloat) 0, (nfloat) 15, (nfloat) 0, (nfloat) 15);
        // ISSUE: reference to a compiler-generated method
        uiButton.SetTitle(ContextActionsCell.GetMoreTranslation(), UIControlState.Normal);
        // ISSUE: reference to a compiler-generated method
        nfloat nfloat = uiButton.TitleLabel.SizeThatFits(new CGSize(width, height)).Width + (nfloat) 30;
        if (nfloat > largestButtonWidth)
        {
          largestButtonWidth = nfloat;
          this.CullButtons(acceptableTotalWidth, ref needMoreButton, ref largestButtonWidth);
          if (largestButtonWidth * (nfloat) 2 > acceptableTotalWidth)
          {
            largestButtonWidth = acceptableTotalWidth / (nfloat) 2;
            flag = true;
          }
        }
        uiButton.Tag = (nint) -1;
        uiButton.TouchUpInside += new EventHandler(this.OnButtonActivated);
        if (flag)
          uiButton.TitleLabel.AdjustsFontSizeToFitWidth = true;
        this.moreButton = uiButton;
        this.buttons.Add(uiButton);
      }
      PropertyChangedEventHandler changedEventHandler = new PropertyChangedEventHandler(this.OnMenuItemPropertyChanged);
      nfloat nfloat1 = (nfloat) this.buttons.Count * largestButtonWidth;
      for (int index = 0; index < this.buttons.Count; ++index)
      {
        UIButton uiButton = this.buttons[index];
        if (uiButton.Tag >= (nint) 0)
          this.cell.get_ContextActions()[(int) uiButton.Tag].add_PropertyChanged(changedEventHandler);
        nfloat nfloat2 = (nfloat) (index + 1) * largestButtonWidth;
        nfloat x = width - nfloat2;
        if (UIDevice.CurrentDevice.CheckSystemVersion(8, 0))
          x += nfloat1;
        uiButton.Frame = new CGRect(x, (nfloat) 0, largestButtonWidth, height);
        if (flag)
          uiButton.TitleLabel.AdjustsFontSizeToFitWidth = true;
        // ISSUE: reference to a compiler-generated method
        uiButton.SetNeedsLayout();
        if (uiButton != this.moreButton)
          uiButton.TouchUpInside += new EventHandler(this.OnButtonActivated);
      }
      return (UIView) null;
    }

    private nfloat GetLargestWidth()
    {
      nfloat nfloat = (nfloat) 0;
      for (int index = 0; index < this.buttons.Count; ++index)
      {
        CGRect frame = this.buttons[index].Frame;
        if (frame.Width > nfloat)
          nfloat = frame.Width;
      }
      return nfloat;
    }

    private void CullButtons(nfloat acceptableTotalWidth, ref bool needMoreButton, ref nfloat largestButtonWidth)
    {
      while (largestButtonWidth * (nfloat) (this.buttons.Count + (needMoreButton ? 1 : 0)) > acceptableTotalWidth && this.buttons.Count > 1)
      {
        needMoreButton = true;
        UIButton uiButton = this.buttons[this.buttons.Count - 1];
        this.buttons.RemoveAt(this.buttons.Count - 1);
        if (largestButtonWidth == uiButton.Frame.Width)
          largestButtonWidth = this.GetLargestWidth();
      }
      if (!needMoreButton || this.cell.get_ContextActions().Count - this.buttons.Count != 1)
        return;
      this.buttons.RemoveAt(this.buttons.Count - 1);
    }

    private void OnButtonActivated(object sender, EventArgs e)
    {
      UIButton uiButton = (UIButton) sender;
      if (uiButton.Tag == (nint) -1)
      {
        this.ActivateMore();
      }
      else
      {
        // ISSUE: reference to a compiler-generated method
        this.scroller.SetContentOffset(new CGPoint((nfloat) 0, (nfloat) 0), true);
        this.cell.get_ContextActions()[(int) uiButton.Tag].Activate();
      }
    }

    private void ActivateMore()
    {
      HashSet<nint> hashSet = new HashSet<nint>();
      for (int index = 0; index < this.buttons.Count; ++index)
      {
        nint tag = this.buttons[index].Tag;
        if (tag >= (nint) 0)
          hashSet.Add(tag);
      }
      CGRect cgRect1 = this.moreButton.Frame;
      if (!Forms.IsiOS8OrNewer)
        cgRect1 = new CGRect(this.moreButton.Superview.Frame.X, (nfloat) 0, cgRect1.Width, cgRect1.Height);
      nfloat x = cgRect1.X - this.scroller.ContentOffset.X;
      // ISSUE: reference to a compiler-generated method
      // ISSUE: reference to a compiler-generated method
      CGRect cgRect2 = this.tableView.RectForRowAtIndexPath(this.tableView.IndexPathForCell((UITableViewCell) this));
      CGRect rect = new CGRect(x, cgRect2.Y, cgRect2.Width, cgRect2.Height);
      if (UIDevice.CurrentDevice.CheckSystemVersion(8, 0))
      {
        ContextActionsCell.MoreActionSheetController actionSheetController1 = new ContextActionsCell.MoreActionSheetController();
        for (int index = 0; index < this.cell.get_ContextActions().Count; ++index)
        {
          if (!hashSet.Contains((nint) index))
          {
            MenuItem target1 = this.cell.get_ContextActions()[index];
            WeakReference<MenuItem> weakItem = new WeakReference<MenuItem>(target1);
            // ISSUE: reference to a compiler-generated method
            UIAlertAction action = UIAlertAction.Create(target1.Text, UIAlertActionStyle.Default, (Action<UIAlertAction>) (a =>
            {
              // ISSUE: reference to a compiler-generated method
              this.scroller.SetContentOffset(new CGPoint((nfloat) 0, (nfloat) 0), true);
              MenuItem target;
              if (!weakItem.TryGetTarget(out target))
                return;
              target.Activate();
            }));
            // ISSUE: reference to a compiler-generated method
            actionSheetController1.AddAction(action);
          }
        }
        UIViewController controller = this.GetController();
        if (controller == null)
          throw new InvalidOperationException("No UIViewController found to present.");
        if (UIDevice.CurrentDevice.UserInterfaceIdiom == UIUserInterfaceIdiom.Phone)
        {
          // ISSUE: reference to a compiler-generated method
          UIAlertAction action = UIAlertAction.Create(ContextActionsCell.GetCancelTranslation(), UIAlertActionStyle.Cancel, (Action<UIAlertAction>) null);
          // ISSUE: reference to a compiler-generated method
          actionSheetController1.AddAction(action);
        }
        else
        {
          actionSheetController1.PopoverPresentationController.SourceView = (UIView) this.tableView;
          actionSheetController1.PopoverPresentationController.SourceRect = rect;
        }
        ContextActionsCell.MoreActionSheetController actionSheetController2 = actionSheetController1;
        int num = 1;
        // ISSUE: variable of the null type
        __Null local = null;
        // ISSUE: reference to a compiler-generated method
        controller.PresentViewController((UIViewController) actionSheetController2, num != 0, (Action) local);
      }
      else
      {
        ContextActionsCell.MoreActionSheetDelegate actionSheetDelegate = new ContextActionsCell.MoreActionSheetDelegate()
        {
          Scroller = this.scroller,
          Items = new List<MenuItem>()
        };
        UIActionSheet uiActionSheet = new UIActionSheet((string) null, (UIActionSheetDelegate) actionSheetDelegate);
        for (int index = 0; index < this.cell.get_ContextActions().Count; ++index)
        {
          if (!hashSet.Contains((nint) index))
          {
            MenuItem menuItem = this.cell.get_ContextActions()[index];
            actionSheetDelegate.Items.Add(menuItem);
            // ISSUE: reference to a compiler-generated method
            uiActionSheet.AddButton(menuItem.Text);
          }
        }
        if (UIDevice.CurrentDevice.UserInterfaceIdiom == UIUserInterfaceIdiom.Phone)
        {
          // ISSUE: reference to a compiler-generated method
          nint nint = uiActionSheet.AddButton(ContextActionsCell.GetCancelTranslation());
          uiActionSheet.CancelButtonIndex = nint;
        }
        // ISSUE: reference to a compiler-generated method
        uiActionSheet.ShowFrom(rect, (UIView) this.tableView, true);
      }
    }

    private UIViewController GetController()
    {
      for (Element element = (Element) this.cell; element.Parent != null; element = element.Parent)
      {
        IVisualElementRenderer renderer = Platform.GetRenderer((VisualElement) element.Parent);
        if (renderer.ViewController != null)
          return renderer.ViewController;
      }
      return (UIViewController) null;
    }

    private UIButton GetButton(MenuItem item)
    {
      UIButton uiButton = new UIButton(new CGRect((nfloat) 0, (nfloat) 0, (nfloat) 1, (nfloat) 1));
      if (!item.IsDestructive)
      {
        // ISSUE: reference to a compiler-generated method
        uiButton.SetBackgroundImage(ContextActionsCell.normalBackground, UIControlState.Normal);
      }
      else
      {
        // ISSUE: reference to a compiler-generated method
        uiButton.SetBackgroundImage(ContextActionsCell.destructiveBackground, UIControlState.Normal);
      }
      // ISSUE: reference to a compiler-generated method
      uiButton.SetTitle(item.Text, UIControlState.Normal);
      uiButton.TitleEdgeInsets = new UIEdgeInsets((nfloat) 0, (nfloat) 15, (nfloat) 0, (nfloat) 15);
      uiButton.Enabled = item.IsEnabled;
      return uiButton;
    }

    private void OnContextItemsChanged(object sender, NotifyCollectionChangedEventArgs e)
    {
      ListView listView = this.cell.Parent as ListView;
      if ((listView == null ? 0 : (listView.CachingStrategy == ListViewCachingStrategy.RecycleElement ? 1 : 0)) != 0)
        this.Update(this.tableView, this.cell, this.ContentCell);
      else
        this.ReloadRow();
    }

    private void OnCellPropertyChanged(object sender, PropertyChangedEventArgs e)
    {
      if (!(e.PropertyName == "HasContextActions"))
        return;
      ListView listView = this.cell.Parent as ListView;
      if ((listView == null ? 0 : (listView.CachingStrategy == ListViewCachingStrategy.RecycleElement ? 1 : 0)) != 0)
        return;
      this.ReloadRow();
    }

    private void OnMenuItemPropertyChanged(object sender, PropertyChangedEventArgs e)
    {
      ListView listView = this.cell.Parent as ListView;
      if ((listView == null ? 0 : (listView.CachingStrategy == ListViewCachingStrategy.RecycleElement ? 1 : 0)) != 0)
        this.Update(this.tableView, this.cell, this.ContentCell);
      else
        this.ReloadRow();
    }

    public void Close()
    {
      this.scroller.ContentOffset = new CGPoint((nfloat) 0, (nfloat) 0);
    }

    private void ReloadRow()
    {
      if (this.scroller.ContentOffset.X > (nfloat) 0)
      {
        ((ContextScrollViewDelegate) this.scroller.Delegate).ClosedCallback = (Action) (() =>
        {
          this.ReloadRowCore();
          ((ContextScrollViewDelegate) this.scroller.Delegate).ClosedCallback = (Action) null;
        });
        // ISSUE: reference to a compiler-generated method
        this.scroller.SetContentOffset(new CGPoint((nfloat) 0, (nfloat) 0), true);
      }
      else
        this.ReloadRowCore();
    }

    private void ReloadRowCore()
    {
      if (this.cell.Parent == null)
        return;
      NSIndexPath indexPath = CellExtensions.GetIndexPath(this.cell);
      bool flag = indexPath.Equals((NSObject) this.tableView.IndexPathForSelectedRow);
      // ISSUE: reference to a compiler-generated method
      this.tableView.ReloadRows(new NSIndexPath[1]
      {
        indexPath
      }, UITableViewRowAnimation.None);
      if (!flag)
        return;
      // ISSUE: reference to a compiler-generated method
      this.tableView.SelectRow(indexPath, false, UITableViewScrollPosition.None);
      // ISSUE: reference to a compiler-generated method
      this.tableView.Source.RowSelected(this.tableView, indexPath);
    }

    private static string GetMoreTranslation()
    {
      string s = NSLocale.PreferredLanguages[0].ToLower();
      // ISSUE: reference to a compiler-generated method
      uint stringHash = \u003CPrivateImplementationDetails\u003E.ComputeStringHash(s);
      if (stringHash <= 1437583224U)
      {
        if (stringHash <= 1162757945U)
        {
          if (stringHash <= 1045564875U)
          {
            if (stringHash <= 926444256U)
            {
              if ((int) stringHash != 550294723)
              {
                if ((int) stringHash == 926444256 && s == "id")
                  return "Lainnya";
              }
              else if (s == "zh-hk")
                goto label_81;
            }
            else if ((int) stringHash != 1011465184)
            {
              if ((int) stringHash == 1045564875 && s == "sk")
                return "Viac";
            }
            else if (s == "vi")
              return "Thêm";
          }
          else if (stringHash <= 1092248970U)
          {
            if ((int) stringHash != 1058693732)
            {
              if ((int) stringHash != 1092248970 || s == "en")
                ;
            }
            else if (s == "el")
              return "Λοιπά";
          }
          else if ((int) stringHash != 1095059089)
          {
            if ((int) stringHash != 1111292255)
            {
              if ((int) stringHash == 1162757945 && s == "pl")
                return "Więcej";
            }
            else if (s == "ko")
              return "기타";
          }
          else if (s == "th")
            return "เพิ่มเติม";
        }
        else if (stringHash <= 1195724803U)
        {
          if (stringHash <= 1176137065U)
          {
            if ((int) stringHash != 1171357212)
            {
              if ((int) stringHash == 1176137065 && s == "es")
                goto label_79;
            }
            else if (s == "fr-ca")
              goto label_78;
          }
          else if ((int) stringHash != 1177122803)
          {
            if ((int) stringHash != 1194886160)
            {
              if ((int) stringHash == 1195724803 && s == "tr")
                return "Diğer";
            }
            else if (s == "it")
              return "Altro";
          }
          else if (s == "cs")
            return "Další";
        }
        else if (stringHash <= 1230118684U)
        {
          if ((int) stringHash != 1213488160)
          {
            if ((int) stringHash == 1230118684 && s == "sv")
              goto label_89;
          }
          else if (s == "ru")
            return "Еще";
        }
        else if ((int) stringHash != 1278921350)
        {
          if ((int) stringHash != 1329254207)
          {
            if ((int) stringHash == 1437583224 && s == "pt-pt")
              goto label_86;
          }
          else if (s == "hr")
            return "Više";
        }
        else if (s == "hu")
          return "Továbbiak";
      }
      else if (stringHash <= 1578064273U)
      {
        if (stringHash <= 1479119945U)
        {
          if (stringHash <= 1461901041U)
          {
            if ((int) stringHash != 1445858897)
            {
              if ((int) stringHash == 1461901041 && s == "fr")
                goto label_78;
            }
            else if (s == "ms")
              return "Lagi";
          }
          else if ((int) stringHash != 1463180969)
          {
            if ((int) stringHash != 1478281302)
            {
              if ((int) stringHash == 1479119945 && s == "ca")
                return "Més";
            }
            else if (s == "da")
              return "Mere";
          }
          else if (s == "nb")
            goto label_89;
        }
        else if (stringHash <= 1545789136U)
        {
          if ((int) stringHash != 1545391778)
          {
            if ((int) stringHash == 1545789136 && s == "fi")
              return "Muut";
          }
          else if (s == "de")
            return "Mehr";
        }
        else if ((int) stringHash != 1547363254)
        {
          if ((int) stringHash != 1565420801)
          {
            if ((int) stringHash == 1578064273 && s == "es-mx")
              goto label_79;
          }
          else if (s == "pt")
            goto label_86;
        }
        else if (s == "he")
          return "עוד";
      }
      else if (stringHash <= 1816099348U)
      {
        if (stringHash <= 1630957159U)
        {
          if ((int) stringHash != 1581462945)
          {
            if ((int) stringHash == 1630957159 && s == "nl")
              return "Meer";
          }
          else if (s == "uk")
            return "Щe";
        }
        else if ((int) stringHash != 1649706254)
        {
          if ((int) stringHash != 1748694682)
          {
            if ((int) stringHash == 1816099348 && s == "ja")
              return "その他";
          }
          else if (s == "hi")
            return "अधिक";
        }
        else if (s == "ro")
          return "Altele";
      }
      else if (stringHash <= 2281825994U)
      {
        if ((int) stringHash != -2097029397)
        {
          if ((int) stringHash == -2013141302 && s == "zh-hans")
            goto label_81;
        }
        else if (s == "zh-hant")
          goto label_81;
      }
      else if ((int) stringHash != -1192991348)
      {
        if ((int) stringHash != -1124306754)
        {
          if ((int) stringHash != -739701445 || s == "en-au")
            ;
        }
        else if (s == "en-in")
          ;
      }
      else if (s == "en-gb")
        ;
      return "More";
label_78:
      return "Options";
label_79:
      return "Más";
label_81:
      return "更多";
label_86:
      return "Mais";
label_89:
      return "Mer";
    }

    private static string GetCancelTranslation()
    {
      string s = NSLocale.PreferredLanguages[0].ToLower();
      // ISSUE: reference to a compiler-generated method
      uint stringHash = \u003CPrivateImplementationDetails\u003E.ComputeStringHash(s);
      if (stringHash <= 1445858897U)
      {
        if (stringHash <= 1171357212U)
        {
          if (stringHash <= 1058693732U)
          {
            if (stringHash <= 926444256U)
            {
              if ((int) stringHash != 550294723)
              {
                if ((int) stringHash == 926444256 && s == "id")
                  return "Batalkan";
              }
              else if (s == "zh-hk")
                goto label_83;
            }
            else if ((int) stringHash != 1011465184)
            {
              if ((int) stringHash != 1045564875)
              {
                if ((int) stringHash == 1058693732 && s == "el")
                  return "Ακύρωση";
              }
              else if (s == "sk")
                return "Zrušiť";
            }
            else if (s == "vi")
              return "Hủy";
          }
          else if (stringHash <= 1095059089U)
          {
            if ((int) stringHash != 1092248970)
            {
              if ((int) stringHash == 1095059089 && s == "th")
                return "ยกเลิก";
            }
            else if (s == "en")
              ;
          }
          else if ((int) stringHash != 1111292255)
          {
            if ((int) stringHash != 1162757945)
            {
              if ((int) stringHash == 1171357212 && s == "fr-ca")
                goto label_80;
            }
            else if (s == "pl")
              return "Anuluj";
          }
          else if (s == "ko")
            return "취소";
        }
        else if (stringHash <= 1213488160U)
        {
          if (stringHash <= 1177122803U)
          {
            if ((int) stringHash != 1176137065)
            {
              if ((int) stringHash == 1177122803 && s == "cs")
                return "Zrušit";
            }
            else if (s == "es")
              goto label_81;
          }
          else if ((int) stringHash != 1194886160)
          {
            if ((int) stringHash != 1195724803)
            {
              if ((int) stringHash == 1213488160 && s == "ru")
                return "Oтменить";
            }
            else if (s == "tr")
              return "Vazgeç";
          }
          else if (s == "it")
            return "Annulla";
        }
        else if (stringHash <= 1278921350U)
        {
          if ((int) stringHash != 1230118684)
          {
            if ((int) stringHash == 1278921350 && s == "hu")
              return "Mégsem";
          }
          else if (s == "sv")
            goto label_90;
        }
        else if ((int) stringHash != 1329254207)
        {
          if ((int) stringHash != 1437583224)
          {
            if ((int) stringHash == 1445858897 && s == "ms")
              return "Batal";
          }
          else if (s == "pt-pt")
            goto label_81;
        }
        else if (s == "hr")
          return "Poništi";
      }
      else if (stringHash <= 1578064273U)
      {
        if (stringHash <= 1545391778U)
        {
          if (stringHash <= 1463180969U)
          {
            if ((int) stringHash != 1461901041)
            {
              if ((int) stringHash == 1463180969 && s == "nb")
                goto label_90;
            }
            else if (s == "fr")
              goto label_80;
          }
          else if ((int) stringHash != 1478281302)
          {
            if ((int) stringHash != 1479119945)
            {
              if ((int) stringHash == 1545391778 && s == "de")
                return "Abbrechen";
            }
            else if (s == "ca")
              return "Cancel·lar";
          }
          else if (s == "da")
            return "Annuller";
        }
        else if (stringHash <= 1547363254U)
        {
          if ((int) stringHash != 1545789136)
          {
            if ((int) stringHash == 1547363254 && s == "he")
              return "ביטול";
          }
          else if (s == "fi")
            return "Kumoa";
        }
        else if ((int) stringHash != 1562713850)
        {
          if ((int) stringHash != 1565420801)
          {
            if ((int) stringHash == 1578064273 && s == "es-mx")
              goto label_81;
          }
          else if (s == "pt")
            goto label_81;
        }
        else if (s == "ar")
          return "إلخاء";
      }
      else if (stringHash <= 1816099348U)
      {
        if (stringHash <= 1630957159U)
        {
          if ((int) stringHash != 1581462945)
          {
            if ((int) stringHash == 1630957159 && s == "nl")
              return "Annuleer";
          }
          else if (s == "uk")
            return "Скасувати";
        }
        else if ((int) stringHash != 1649706254)
        {
          if ((int) stringHash != 1748694682)
          {
            if ((int) stringHash == 1816099348 && s == "ja")
              return "キャンセル";
          }
          else if (s == "hi")
            return "रद्द करें";
        }
        else if (s == "ro")
          return "Anulați";
      }
      else if (stringHash <= 2281825994U)
      {
        if ((int) stringHash != -2097029397)
        {
          if ((int) stringHash == -2013141302 && s == "zh-hans")
            goto label_83;
        }
        else if (s == "zh-hant")
          goto label_83;
      }
      else if ((int) stringHash != -1192991348)
      {
        if ((int) stringHash != -1124306754)
        {
          if ((int) stringHash != -739701445 || s == "en-au")
            ;
        }
        else if (s == "en-in")
          ;
      }
      else if (s == "en-gb")
        ;
      return "Cancel";
label_80:
      return "Annuler";
label_81:
      return "Cancelar";
label_83:
      return "取消";
label_90:
      return "Avbryt";
    }

    private class SelectGestureRecognizer : UITapGestureRecognizer
    {
      private NSIndexPath lastPath;

      public SelectGestureRecognizer()
        : base(new Action<UITapGestureRecognizer>(ContextActionsCell.SelectGestureRecognizer.Tapped))
      {
        this.ShouldReceiveTouch = (UITouchEventArgs) ((recognizer, touch) =>
        {
          UITableView uiTableView = (UITableView) this.View;
          // ISSUE: reference to a compiler-generated method
          CGPoint point = touch.LocationInView((UIView) uiTableView);
          // ISSUE: reference to a compiler-generated method
          this.lastPath = uiTableView.IndexPathForRowAtPoint(point);
          if (this.lastPath == null)
            return false;
          // ISSUE: reference to a compiler-generated method
          return uiTableView.CellAt(this.lastPath) is ContextActionsCell;
        });
      }

      private static void Tapped(UIGestureRecognizer recognizer)
      {
        ContextActionsCell.SelectGestureRecognizer gestureRecognizer = (ContextActionsCell.SelectGestureRecognizer) recognizer;
        UITableView tableView = (UITableView) recognizer.View;
        if (!gestureRecognizer.lastPath.Equals((NSObject) tableView.IndexPathForSelectedRow))
        {
          // ISSUE: reference to a compiler-generated method
          tableView.SelectRow(gestureRecognizer.lastPath, false, UITableViewScrollPosition.None);
        }
        // ISSUE: reference to a compiler-generated method
        tableView.Source.RowSelected(tableView, gestureRecognizer.lastPath);
      }
    }

    private class MoreActionSheetController : UIAlertController
    {
      public override UIAlertControllerStyle PreferredStyle
      {
        get
        {
          return UIAlertControllerStyle.ActionSheet;
        }
      }

      public override void WillRotate(UIInterfaceOrientation toInterfaceOrientation, double duration)
      {
        // ISSUE: reference to a compiler-generated method
        this.DismissViewController(false, (Action) null);
      }
    }

    private class MoreActionSheetDelegate : UIActionSheetDelegate
    {
      public List<MenuItem> Items;
      public UIScrollView Scroller;

      public override void Clicked(UIActionSheet actionSheet, nint buttonIndex)
      {
        if (buttonIndex == (nint) this.Items.Count)
          return;
        // ISSUE: reference to a compiler-generated method
        this.Scroller.SetContentOffset(new CGPoint((nfloat) 0, (nfloat) 0), true);
        if (!(buttonIndex >= (nint) 0))
          return;
        this.Items[(int) buttonIndex].Activate();
      }
    }
  }
}
