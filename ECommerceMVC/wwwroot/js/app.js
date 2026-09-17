const apiUrl = "http://localhost:5006/Product";
function getProductImage(productName) {

    const name = productName.toLowerCase();

    if (name.includes("bisiklet")) {
        return "wwwroot/images/bisiklet.jpg";
    }

    if (name.includes("duvar saati")) {
        return "wwwroot/images/duvar-saati.jpg";
    }

    if (name.includes("mouse")) {
        return "wwwroot/images/mouse.jpg";
    }

    if (name.includes("masa")) {
        return "wwwroot/images/masa.jpg";
    }

    if (name.includes("laptop")) {
        return "wwwroot/images/laptop.jpg";
    }

    if (name.includes("yazıcı")) {
        return "wwwroot/images/yazici.jpg";
    }

    if (name.includes("powerbank")) {
        return "wwwroot/images/powerbank.jpg";
    }

    if (name.includes("ofis sandalyesi")) {
        return "wwwroot/images/ofis-sandalyesi.jpg";
    }

    if (name.includes("akıllı saat")) {
        return "wwwroot/images/akilli-saat.jpg";
    }

    if (name.includes("tablet")) {
        return "wwwroot/images/tablet.jpg";
    }

    if (name.includes("telefon")) {
        return "wwwroot/images/telefon.jpg";
    }

    if (name.includes("ssd")) {
        return "wwwroot/images/ssd.jpg";
    }

    if (name.includes("kulaklık")) {
        return "wwwroot/images/kulaklik.jpg";
    }

    if (name.includes("kamera")) {
        return "wwwroot/images/kamera.jpg";
    }

    if (name.includes("televizyon")) {
        return "wwwroot/images/televizyon.jpg";
    }

    if (name.includes("konsol")) {
        return "wwwroot/images/konsol.jpg";
    }

    return "wwwroot/images/default.jpg";
}

document.addEventListener("DOMContentLoaded", () => {
  getProducts();
});

async function getProducts() {

    const tbody = document.getElementById("productTableBody");

    try {

        console.log("API isteği gönderiliyor:", apiUrl);

        const response = await fetch(apiUrl);

        console.log("API durum kodu:", response.status);

        if (!response.ok) {
            throw new Error("API bağlantı hatası: " + response.status);
        }

        const data = await response.json();

        console.log("Ürün verisi:", data);

        if (!Array.isArray(data)) {
            throw new Error("API'den ürün listesi gelmedi.");
        }

        if (data.length === 0) {

            tbody.innerHTML = `
                <tr>
                    <td colspan="5" class="text-center text-warning">
                        Henüz ürün bulunmuyor.
                    </td>
                </tr>
            `;

            return;
        }

        // Önce bütün HTML'i oluşturuyoruz
        let html = "";

        data.forEach(product => {

            html += `
                <tr>

                    <td class="align-middle">
                       <img 
    src="${getProductImage(product.name)}"
    class="img-thumbnail"
    style="width:70px; height:70px; object-fit:contain;"
>
                    </td>

                    <td class="align-middle font-weight-bold text-dark">
                        ${product.name || product.title || "Ürün adı yok"}
                    </td>

                    <td class="align-middle text-muted small">
                        ${product.description || "Açıklama mevcut değil."}
                    </td>

                    <td class="align-middle text-danger font-weight-bold">
                        ${product.price} ₺
                    </td>

                    <td class="align-middle text-center">
                        <button 
                            class="btn btn-primary btn-sm px-3"
                            onclick="addToCart(${product.id})">
                            🛒 Satın Al
                        </button>
                    </td>

                </tr>
            `;
        });

        // HTML'i tek seferde tabloya ekliyoruz
        tbody.innerHTML = html;

        console.log(data.length + " ürün tabloya yüklendi.");

    }
    catch (error) {

        console.error("Ürün yükleme hatası:", error);

        tbody.innerHTML = `
            <tr>
                <td colspan="5" class="text-center text-danger font-weight-bold">
                    Ürünler yüklenemedi.
                </td>
            </tr>
        `;
    }
}


let count = 0;

function addToCart(id) {

    count++;

    document.getElementById("cartCount").innerText = count;

    alert("Ürün sepetinize eklendi!");
}