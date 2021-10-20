((wnd, doc) => {
 
  wnd['q2a'] = (q, p) => Array.from((p || doc).querySelectorAll(q));

})(window, document);
