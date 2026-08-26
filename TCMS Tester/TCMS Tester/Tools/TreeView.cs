using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Runtime.InteropServices;
using System.Windows.Forms;

public class ModernTreeView : TreeView
{
    // Win32 내부 더블 버퍼링 처리를 위한 메시지 송신 API
    [DllImport("user32.dll", CharSet = CharSet.Auto)]
    private static extern IntPtr SendMessage(IntPtr hWnd, int msg, IntPtr wp, IntPtr lp);
    private const int TVM_SETEXTENDEDSTYLE = 0x1100 + 44;
    private const int TVS_EX_DOUBLEBUFFER = 0x0004;

    // 레이아웃 배치를 위한 고정 픽셀 상수 정의
    private const int nIndentPerLevel = 25;
    private const int nBaseIndent = 10;
    private const int nCheckOffsetX = 5;
    private const int nCheckSize = 18;       // 요구사항 반영: 체크박스 크기를 14px에서 18px로 확대
    private const int nTextGap = 8;          // 늘어난 크기에 맞추어 공백 밸런스 조정
    private const int nIconGap = 10;
    private const int nIconSize = 16;

    // 상태 제어 및 설정 플래그 변수
    private bool bIsUpdating = false;
    private bool bAutoExpandAllOnLoad = false;

    // 그래픽 자원 소모 최소화를 위한 GDI 객체 캐시 필드
    private SolidBrush brushBg;
    private SolidBrush brushText;
    private SolidBrush brushCheckBg;
    private Pen penCheck;
    private Pen penUnchecked;
    private Pen penIconBorder;
    private Pen penIconShape;

    // 숨김 처리된 노드들을 안전하게 추적하기 위한 데이터 저장소
    private readonly Dictionary<string, Tuple<TreeNodeCollection, int, TreeNode>> _hiddenNodes =
        new Dictionary<string, Tuple<TreeNodeCollection, int, TreeNode>>();

    // 렌더링 좌표 공유를 위한 불변 레이아웃 구조체
    private readonly struct NodeLayout
    {
        public readonly Rectangle rectCheck;
        public readonly int nTextX;
        public readonly Rectangle rectIcon;

        public NodeLayout(Rectangle rectCheck, int nTextX, Rectangle rectIcon)
        {
            this.rectCheck = rectCheck;
            this.nTextX = nTextX;
            this.rectIcon = rectIcon;
        }
    }

    public ModernTreeView()
    {
        this.DrawMode = TreeViewDrawMode.OwnerDrawAll;
        this.ShowLines = false;
        this.ShowPlusMinus = false;
        this.FullRowSelect = true;

        // 요구사항 반영: 노드 간격 분리를 위해 기존 38px에서 46px로 수직 높이 대폭 확장
        this.ItemHeight = 56;

        this.Font = new Font("맑은 고딕", 10F, FontStyle.Regular);

        // 흰색 배경 기반 프로젝트에 녹아들도록 디폴트 테마 색상 반전 처리
        this.BackColor = Color.White;
        this.ForeColor = Color.FromArgb(40, 45, 55);
        this.BorderStyle = BorderStyle.None;
        if (this.IsHandleCreated)
        {
            this.RecreateHandle();
        }
        InitGdiCache();
    }

    [Category("Behavior")]
    [Description("true로 설정하면 폼이 처음 로드되거나 핸들이 생성될 때 모든 노드를 자동으로 펼칩니다.")]
    [DefaultValue(false)]
    public bool AutoExpandAllOnLoad
    {
        get => bAutoExpandAllOnLoad;
        set
        {
            bAutoExpandAllOnLoad = value;

            if (LicenseManager.UsageMode == LicenseUsageMode.Designtime)
                return;

            if (bAutoExpandAllOnLoad && this.IsHandleCreated && !this.DesignMode)
            {
                this.ExpandAll();
            }
        }
    }

    // 변경된 흰색 배경 사양에 맞추어 그래픽 펜 및 브러시 명암비 재조정
    private void InitGdiCache()
    {
        DisposeGdiCache();

        brushBg = new SolidBrush(this.BackColor);
        brushText = new SolidBrush(this.ForeColor);
        brushCheckBg = new SolidBrush(Color.White);

        // 체크 내부 V마크 굵기를 크기에 비례해 2.5f로 강화
        penCheck = new Pen(Color.FromArgb(0, 140, 240), 2.5f);
        penUnchecked = new Pen(Color.FromArgb(175, 185, 200), 1.5f);

        // 요구사항 반영: 흰색 배경에서 흐릿하던 확장 아이콘 테두리와 내부 기호를 선명한 스카이 블루/그레이 조합으로 변경
        penIconBorder = new Pen(Color.FromArgb(190, 200, 215), 1.5f);
        penIconShape = new Pen(Color.FromArgb(255, 128, 0), 2.0f);
    }

    private void DisposeGdiCache()
    {
        brushBg?.Dispose();
        brushText?.Dispose();
        brushCheckBg?.Dispose();
        penCheck?.Dispose();
        penUnchecked?.Dispose();
        penIconBorder?.Dispose();
        penIconShape?.Dispose();
    }

    protected override void OnForeColorChanged(EventArgs e)
    {
        base.OnForeColorChanged(e);
        brushText?.Dispose();
        brushText = new SolidBrush(this.ForeColor);
    }

    protected override void OnBackColorChanged(EventArgs e)
    {
        base.OnBackColorChanged(e);
        brushBg?.Dispose();
        brushBg = new SolidBrush(this.BackColor);
    }

    protected override void OnHandleCreated(EventArgs e)
    {
        base.OnHandleCreated(e);

        SendMessage(this.Handle, TVM_SETEXTENDEDSTYLE, (IntPtr)TVS_EX_DOUBLEBUFFER, (IntPtr)TVS_EX_DOUBLEBUFFER);

        if (LicenseManager.UsageMode == LicenseUsageMode.Designtime || this.DesignMode)
            return;

        if (bAutoExpandAllOnLoad)
        {
            this.BeginUpdate();
            this.ExpandAll();
            this.EndUpdate();
        }
    }

    protected override void OnParentChanged(EventArgs e)
    {
        base.OnParentChanged(e);
        if (Parent != null)
        {
            Parent.Click -= Parent_Click;
            Parent.Click += Parent_Click;
        }
    }

    private void Parent_Click(object sender, EventArgs e)
    {
        this.Focus();
    }

    // 확대된 체크박스와 행 높이에 어긋나지 않도록 정밀 수직 중앙 오프셋 연산
    private NodeLayout GetNodeLayout(TreeNode node)
    {
        int nIndent = node.Level * nIndentPerLevel + nBaseIndent;
        int nCheckX = nIndent + nCheckOffsetX;

        Rectangle rectCheck = new Rectangle(
            nCheckX,
            node.Bounds.Top + (this.ItemHeight - nCheckSize) / 2,
            nCheckSize,
            nCheckSize);

        int nTextX = rectCheck.Right + nTextGap;
        Rectangle rectIcon = Rectangle.Empty;

        if (node.Nodes.Count > 0)
        {
            Size sizeText = TextRenderer.MeasureText(node.Text, this.Font);
            int nIconX = nTextX + sizeText.Width + nIconGap;
            rectIcon = new Rectangle(
                nIconX,
                node.Bounds.Top + (this.ItemHeight - nIconSize) / 2,
                nIconSize,
                nIconSize);
        }

        return new NodeLayout(rectCheck, nTextX, rectIcon);
    }

    protected override void OnDrawNode(DrawTreeNodeEventArgs e)
    {
        if (e.Node == null) return;

        // 아이템 전체 행 배경 드로잉
        e.Graphics.FillRectangle(brushBg, e.Bounds);

        NodeLayout layout = GetNodeLayout(e.Node);
        Rectangle rectCheck = layout.rectCheck;

        // 체크박스 배경 외곽선 렌더링
        e.Graphics.FillRectangle(brushCheckBg, rectCheck);
        e.Graphics.DrawRectangle(penUnchecked, rectCheck.X, rectCheck.Y, rectCheck.Width - 1, rectCheck.Height - 1);

        // 체크박스가 커짐에 따라 내부 안티앨리어싱 V자 드로잉 좌표를 대칭 비율로 정밀 수정
        if (e.Node.Checked)
        {
            SmoothingMode modePrev = e.Graphics.SmoothingMode;
            try
            {
                e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;

                PointF ptStart = new PointF(rectCheck.Left + 3.5f, rectCheck.Top + 8.5f);
                PointF ptVertex = new PointF(rectCheck.Left + 7.5f, rectCheck.Bottom - 5.0f);
                PointF ptEnd = new PointF(rectCheck.Right - 3.5f, rectCheck.Top + 4.5f);

                e.Graphics.DrawLine(penCheck, ptStart, ptVertex);
                e.Graphics.DrawLine(penCheck, ptVertex, ptEnd);
            }
            finally
            {
                e.Graphics.SmoothingMode = modePrev;
            }
        }

        // 증가된 높이에 매칭되도록 텍스트 글꼴 중앙 마운트
        Size sizeText = TextRenderer.MeasureText(e.Node.Text, this.Font);
        int nTextY = e.Bounds.Top + (this.ItemHeight - sizeText.Height) / 2;
        e.Graphics.DrawString(e.Node.Text, this.Font, brushText, layout.nTextX, nTextY);

        // 부모 노드 펼침/접힘 상태 기호 렌더링 (흰 배경에서 완벽한 명암비 확보)
        if (!layout.rectIcon.IsEmpty)
        {
            Rectangle rectIcon = layout.rectIcon;
            e.Graphics.DrawRectangle(penIconBorder, rectIcon);

            int nIconCenterY = rectIcon.Top + (rectIcon.Height / 2);
            e.Graphics.DrawLine(penIconShape, rectIcon.Left + 4, nIconCenterY, rectIcon.Right - 4, nIconCenterY);

            if (!e.Node.IsExpanded)
            {
                int nIconCenterX = rectIcon.Left + (rectIcon.Width / 2);
                e.Graphics.DrawLine(penIconShape, nIconCenterX, rectIcon.Top + 4, nIconCenterX, rectIcon.Bottom - 4);
            }
        }
    }

    protected override void OnMouseDown(MouseEventArgs e)
    {
        base.OnMouseDown(e);

        TreeNode node = this.GetNodeAt(e.Location);
        if (node == null) return;

        NodeLayout layout = GetNodeLayout(node);

        if (layout.rectCheck.Contains(e.Location))
        {
            node.Checked = !node.Checked;
            SyncCheckState(node);
        }
        else if (!layout.rectIcon.IsEmpty && layout.rectIcon.Contains(e.Location))
        {
            if (node.IsExpanded) node.Collapse();
            else node.Expand();
        }
        else
        {
            node.Checked = !node.Checked;
            SyncCheckState(node);
        }

        this.Invalidate(node.Bounds);
    }

    protected override void OnAfterCheck(TreeViewEventArgs e)
    {
        if (bIsUpdating) return;
        base.OnAfterCheck(e);
    }

    private void SyncCheckState(TreeNode node)
    {
        if (bIsUpdating) return;
        try
        {
            bIsUpdating = true;
            CheckAllChildrenIterative(node, node.Checked);
            UpdateParentCheckStateIterative(node);
        }
        finally
        {
            bIsUpdating = false;
        }
    }

    private void CheckAllChildrenIterative(TreeNode root, bool isChecked)
    {
        var stack = new Stack<TreeNode>();
        foreach (TreeNode child in root.Nodes)
            stack.Push(child);

        while (stack.Count > 0)
        {
            TreeNode node = stack.Pop();
            node.Checked = isChecked;
            foreach (TreeNode child in node.Nodes)
                stack.Push(child);
        }
    }

    private void UpdateParentCheckStateIterative(TreeNode startNode)
    {
        TreeNode current = startNode.Parent;
        while (current != null)
        {
            bool allChecked = true;
            foreach (TreeNode sibling in current.Nodes)
            {
                if (!sibling.Checked) { allChecked = false; break; }
            }
            current.Checked = allChecked;
            current = current.Parent;
        }
    }

    public void SetNodeVisible(string nodeName, bool visible)
    {
        if (visible)
        {
            if (_hiddenNodes.TryGetValue(nodeName, out var info))
            {
                TreeNodeCollection parentNodes = info.Item1;
                int insertIndex = Math.Min(info.Item2, parentNodes.Count);
                parentNodes.Insert(insertIndex, info.Item3);
                _hiddenNodes.Remove(nodeName);
            }
        }
        else
        {
            var found = this.Nodes.Find(nodeName, true);
            if (found.Length > 0 && !_hiddenNodes.ContainsKey(nodeName))
            {
                TreeNode target = found[0];
                TreeNodeCollection p = target.Parent != null ? target.Parent.Nodes : this.Nodes;
                int idx = p.IndexOf(target);

                _hiddenNodes[nodeName] = Tuple.Create(p, idx, target);
                target.Remove();
            }
        }
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            if (Parent != null)
            {
                Parent.Click -= Parent_Click;
            }
            DisposeGdiCache();
        }
        base.Dispose(disposing);
    }
}