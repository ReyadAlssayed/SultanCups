window.downloadFile = (fileName, base64Data) => {

    const link = document.createElement("a");

    link.href =
        "data:application/vnd.openxmlformats-officedocument.spreadsheetml.sheet;base64," +
        base64Data;

    link.download = fileName;

    document.body.appendChild(link);

    link.click();

    document.body.removeChild(link);
};


// طباعة pdf
window.printPurchaseCard = function () {

    const content = document.querySelector('.sal-dialog-box').outerHTML;

    const css = Array.from(document.querySelectorAll('link[rel="stylesheet"],style'))
        .map(x => x.outerHTML)
        .join('');

    const win = window.open('', '', 'width=900,height=700');



    win.document.write(`
        <html dir="rtl">
        <head>
            ${css}
            <title></title>
            <style>
                @page{
                    size:auto;
                    margin:10mm;
                }

                body{
                    margin:0;
                    padding:0;
                    background:#ffffff;
                }

                .print-head{
                    width:700px;
                    margin:0 auto 18px auto;
                    text-align:right;
                    font-size:16px;
                    font-weight:900;
                    color:#111827;
                }

                .sal-dialog-box{
                    margin:0 auto !important;
                    width:700px !important;
                    box-shadow:none !important;
                }

                .pro-dialog-actions{
                    display:none !important;
                }

                .pro-dialog-overlay{
                    background:none !important;
                }
            </style>

        </head>
        <body>

            <div class="print-head">
                مصنع السلطان للأكواب الورقية
            </div>

            ${content}

        </body>
        </html>
    `);

    win.document.close();
    win.focus();

    setTimeout(() => {
        win.print();
        win.close();
    }, 700);
};