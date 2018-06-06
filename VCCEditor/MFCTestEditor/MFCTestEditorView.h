
// MFCTestEditorView.h: interfaz de la clase CMFCTestEditorView
//

#pragma once


//class CMFCTestEditorView : public CView //CWinFormsView 
class CMFCTestEditorView : public CWinFormsView 
{
protected: // Crear s�lo a partir de serializaci�n
	CMFCTestEditorView();
	DECLARE_DYNCREATE(CMFCTestEditorView)
	
// Atributos
public:
	CMFCTestEditorDoc* GetDocument() const;
	

// Operaciones
public:

// Reemplazos
public:
	virtual void OnDraw(CDC* pDC);  // Reemplazado para dibujar esta vista
	virtual BOOL PreCreateWindow(CREATESTRUCT& cs);
protected:
	virtual BOOL OnPreparePrinting(CPrintInfo* pInfo);
	virtual void OnBeginPrinting(CDC* pDC, CPrintInfo* pInfo);
	virtual void OnEndPrinting(CDC* pDC, CPrintInfo* pInfo);

// Implementaci�n
public:
	virtual ~CMFCTestEditorView();
#ifdef _DEBUG
	virtual void AssertValid() const;
	virtual void Dump(CDumpContext& dc) const;
#endif

protected:

// Funciones de asignaci�n de mensajes generadas
protected:
	afx_msg void OnFilePrintPreview();
	afx_msg void OnRButtonUp(UINT nFlags, CPoint point);
	afx_msg void OnContextMenu(CWnd* pWnd, CPoint point);
	DECLARE_MESSAGE_MAP()
};

#ifndef _DEBUG  // Versi�n de depuraci�n en MFCTestEditorView.cpp
inline CMFCTestEditorDoc* CMFCTestEditorView::GetDocument() const
   { return reinterpret_cast<CMFCTestEditorDoc*>(m_pDocument); }
#endif

