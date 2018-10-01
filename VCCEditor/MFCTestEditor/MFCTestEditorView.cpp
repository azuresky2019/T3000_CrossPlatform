
// MFCTestEditorView.cpp: implementaci�n de la clase CMFCTestEditorView
//

#include "stdafx.h"
// Se pueden definir SHARED_HANDLERS en un proyecto ATL implementando controladores de vista previa, miniatura
// y filtro de b�squeda, y permiten compartir c�digo de documentos con ese proyecto.
#ifndef SHARED_HANDLERS
#include "MFCTestEditor.h"
#endif

#include "MFCTestEditorDoc.h"
#include "MFCTestEditorView.h"
#include "VCCEditBox.h"

#ifdef _DEBUG
#define new DEBUG_NEW
#endif


// CMFCTestEditorView

IMPLEMENT_DYNCREATE(CMFCTestEditorView, CWinFormsView)
//IMPLEMENT_DYNCREATE(CMFCTestEditorView, CView)
	

BEGIN_MESSAGE_MAP(CMFCTestEditorView, CWinFormsView)
	
//BEGIN_MESSAGE_MAP(CMFCTestEditorView, CView)
	// Comandos de impresi�n est�ndar
	ON_COMMAND(ID_FILE_PRINT, &CView::OnFilePrint)
	ON_COMMAND(ID_FILE_PRINT_DIRECT, &CView::OnFilePrint)
	ON_COMMAND(ID_FILE_PRINT_PREVIEW, &CMFCTestEditorView::OnFilePrintPreview)
	ON_WM_CONTEXTMENU()
	ON_WM_RBUTTONUP()
END_MESSAGE_MAP()

// Construcci�n o destrucci�n de CMFCTestEditorView

//CMFCTestEditorView::CMFCTestEditorView(): CView()//CWinFormsView(FastColoredTextBoxNS::IronyFCTB::typeid)   
CMFCTestEditorView::CMFCTestEditorView(): CWinFormsView(MFCTestEditor::VCCEditBox::typeid)   
{


}

CMFCTestEditorView::~CMFCTestEditorView()
{

}

BOOL CMFCTestEditorView::PreCreateWindow(CREATESTRUCT& cs)
{
	// TODO: modificar aqu� la clase Window o los estilos cambiando
	//  CREATESTRUCT cs

	return CView::PreCreateWindow(cs);
}

// Dibujo de CMFCTestEditorView

void CMFCTestEditorView::OnDraw(CDC* /*pDC*/)
{
	CMFCTestEditorDoc* pDoc = GetDocument();
	ASSERT_VALID(pDoc);
	if (!pDoc)
		return;

	// TODO: agregar aqu� el c�digo de dibujo para datos nativos
}


// Impresi�n de CMFCTestEditorView


void CMFCTestEditorView::OnFilePrintPreview()
{
#ifndef SHARED_HANDLERS
	AFXPrintPreview(this);
#endif
}

BOOL CMFCTestEditorView::OnPreparePrinting(CPrintInfo* pInfo)
{
	// Preparaci�n predeterminada
	return DoPreparePrinting(pInfo);
}

void CMFCTestEditorView::OnBeginPrinting(CDC* /*pDC*/, CPrintInfo* /*pInfo*/)
{
	// TODO: agregar inicializaci�n adicional antes de imprimir
}

void CMFCTestEditorView::OnEndPrinting(CDC* /*pDC*/, CPrintInfo* /*pInfo*/)
{
	// TODO: agregar limpieza despu�s de imprimir
}

void CMFCTestEditorView::OnRButtonUp(UINT /* nFlags */, CPoint point)
{
	ClientToScreen(&point);
	OnContextMenu(this, point);
}

void CMFCTestEditorView::OnContextMenu(CWnd* /* pWnd */, CPoint point)
{
#ifndef SHARED_HANDLERS
	theApp.GetContextMenuManager()->ShowPopupMenu(IDR_POPUP_EDIT, point.x, point.y, this, TRUE);
#endif
}


// Diagn�sticos de CMFCTestEditorView

#ifdef _DEBUG
void CMFCTestEditorView::AssertValid() const
{
	CView::AssertValid();
}

void CMFCTestEditorView::Dump(CDumpContext& dc) const
{
	CView::Dump(dc);
}

CMFCTestEditorDoc* CMFCTestEditorView::GetDocument() const // La versi�n de no depuraci�n es en l�nea
{
	ASSERT(m_pDocument->IsKindOf(RUNTIME_CLASS(CMFCTestEditorDoc)));
	return (CMFCTestEditorDoc*)m_pDocument;
}
#endif //_DEBUG


// Controladores de mensaje de CMFCTestEditorView
