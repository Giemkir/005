/**
 * YerelTurlar Admin Panel JavaScript
 */

$(document).ready(function() {
    // Menü Açma/Kapama İşlevi
    $('.sidebar-toggle').on('click', function() {
        $('.admin-sidebar').toggleClass('active');
        $('.admin-main').toggleClass('sidebar-open');
    });

    // Mobil Görünümde Menü Kapatma
    $('.admin-sidebar-close').on('click', function() {
        $('.admin-sidebar').removeClass('active');
    });

    // Pencere Boyutu Değiştiğinde
    $(window).resize(function() {
        if ($(window).width() > 992) {
            $('.admin-sidebar').removeClass('active');
        }
    });

    // Bildirim Dropdown
    $('.admin-notification').on('click', function(e) {
        e.stopPropagation();
        $(this).find('.dropdown-menu').toggleClass('show');
        $('.admin-profile').find('.dropdown-menu').removeClass('show');
    });

    // Profil Dropdown
    $('.admin-profile').on('click', function(e) {
        e.stopPropagation();
        $(this).find('.dropdown-menu').toggleClass('show');
        $('.admin-notification').find('.dropdown-menu').removeClass('show');
    });

    // Dropdown Dışı Tıklama
    $(document).on('click', function() {
        $('.dropdown-menu').removeClass('show');
    });

    // Arama İşlevi
    $('.admin-search button').on('click', function() {
        const searchQuery = $('.admin-search input').val().trim();
        if (searchQuery) {
            // Burada arama işlevi gerçekleştirilecek
            console.log('Arama: ' + searchQuery);
        }
    });

    // Enter Tuşuyla Arama
    $('.admin-search input').on('keypress', function(e) {
        if (e.which === 13) {
            e.preventDefault();
            $('.admin-search button').click();
        }
    });

    // DataTable Türkçe Dil Ayarı
    if ($.fn.dataTable) {
        $.extend(true, $.fn.dataTable.defaults, {
            language: {
                url: '//cdn.datatables.net/plug-ins/1.13.4/i18n/tr.json'
            },
            responsive: true,
            autoWidth: false,
            pageLength: 10,
            lengthMenu: [[10, 25, 50, -1], [10, 25, 50, "Tümü"]]
        });

        // Datatables Başlatma
        $('.datatable').DataTable();
    }

    // Bootstrap Tooltip Başlatma
    var tooltipTriggerList = [].slice.call(document.querySelectorAll('[data-bs-toggle="tooltip"]'));
    tooltipTriggerList.map(function (tooltipTriggerEl) {
        return new bootstrap.Tooltip(tooltipTriggerEl);
    });

    // Bootstrap Popover Başlatma
    var popoverTriggerList = [].slice.call(document.querySelectorAll('[data-bs-toggle="popover"]'));
    popoverTriggerList.map(function (popoverTriggerEl) {
        return new bootstrap.Popover(popoverTriggerEl);
    });

    // Silme İşlemi Onayı
    $('.delete-confirm').on('click', function(e) {
        e.preventDefault();
        const itemId = $(this).data('id');
        const itemType = $(this).data('type') || 'öğe';
        
        Swal.fire({
            title: 'Emin misiniz?',
            text: `Bu ${itemType}yi silmek istediğinize emin misiniz?`,
            icon: 'warning',
            showCancelButton: true,
            confirmButtonColor: '#4e73df',
            cancelButtonColor: '#6c757d',
            confirmButtonText: 'Evet, sil!',
            cancelButtonText: 'İptal'
        }).then((result) => {
            if (result.isConfirmed) {
                // Silme formunu gönder
                $(`#delete-form-${itemId}`).submit();
            }
        });
    });

    // Toast Bildirimleri
    function showToast(message, type = 'success') {
        const toastId = 'toast-' + Date.now();
        const toast = `
            <div class="toast-container position-fixed top-0 end-0 p-3">
                <div id="${toastId}" class="toast" role="alert" aria-live="assertive" aria-atomic="true">
                    <div class="toast-header bg-${type} text-white">
                        <strong class="me-auto">YerelTurlar</strong>
                        <small>Şimdi</small>
                        <button type="button" class="btn-close btn-close-white" data-bs-dismiss="toast" aria-label="Kapat"></button>
                    </div>
                    <div class="toast-body">
                        ${message}
                    </div>
                </div>
            </div>
        `;
        
        $('body').append(toast);
        const toastElement = new bootstrap.Toast(document.getElementById(toastId), {
            delay: 5000
        });
        toastElement.show();
        
        // 5 saniye sonra DOM'dan kaldır
        setTimeout(function() {
            $(`#${toastId}`).parent().remove();
        }, 5500);
    }

    // URL'den mesaj parametresini kontrol et
    const urlParams = new URLSearchParams(window.location.search);
    if (urlParams.has('message')) {
        showToast(urlParams.get('message'), urlParams.get('type') || 'success');
    }

    // İstatistik Kartları için Animasyon
    $('.stat-card').each(function(index) {
        const $card = $(this);
        setTimeout(function() {
            $card.addClass('stat-animate');
        }, index * 100);
    });

    // Grafik Oluşturma (Chart.js kullanılıyorsa)
    if (typeof Chart !== 'undefined') {
        // Örnek: Kullanıcı İstatistikleri Grafiği
        if ($('#userStatsChart').length > 0) {
            const ctx = document.getElementById('userStatsChart').getContext('2d');
            const chart = new Chart(ctx, {
                type: 'line',
                data: {
                    labels: ['Ocak', 'Şubat', 'Mart', 'Nisan', 'Mayıs', 'Haziran'],
                    datasets: [{
                        label: 'Yeni Kullanıcılar',
                        data: [12, 19, 3, 5, 2, 3],
                        backgroundColor: 'rgba(78, 115, 223, 0.2)',
                        borderColor: 'rgba(78, 115, 223, 1)',
                        borderWidth: 2,
                        tension: 0.3
                    }]
                },
                options: {
                    responsive: true,
                    maintainAspectRatio: false
                }
            });
        }
        
        // Örnek: Kategori Dağılım Grafiği
        if ($('#categoryChart').length > 0) {
            const ctx = document.getElementById('categoryChart').getContext('2d');
            const chart = new Chart(ctx, {
                type: 'doughnut',
                data: {
                    labels: ['Kültür', 'Doğa', 'Gastronomi', 'Spor', 'Diğer'],
                    datasets: [{
                        data: [30, 25, 20, 15, 10],
                        backgroundColor: [
                            '#4e73df', '#1cc88a', '#36b9cc', '#f6c23e', '#858796'
                        ],
                        hoverBackgroundColor: [
                            '#2e59d9', '#17a673', '#2c9faf', '#f4b619', '#6e707e'
                        ],
                        borderWidth: 0
                    }]
                },
                options: {
                    responsive: true,
                    maintainAspectRatio: false
                }
            });
        }
    }

    // Form Doğrulama
    if (typeof $.validator !== 'undefined') {
        $.validator.setDefaults({
            errorElement: 'div',
            errorClass: 'invalid-feedback',
            highlight: function(element) {
                $(element).addClass('is-invalid').removeClass('is-valid');
            },
            unhighlight: function(element) {
                $(element).addClass('is-valid').removeClass('is-invalid');
            },
            errorPlacement: function(error, element) {
                error.insertAfter(element);
            }
        });
        
        // Form doğrulama başlatma
        $('.needs-validation').validate();
    }

    // Dosya Yükleme Önizleme
    $('.input-file-upload').on('change', function() {
        const file = this.files[0];
        const preview = $(this).data('preview');
        
        if (file && preview) {
            const reader = new FileReader();
            reader.onload = function(e) {
                $(preview).attr('src', e.target.result);
            };
            reader.readAsDataURL(file);
        }
        
        // Dosya adını göster
        const fileName = file ? file.name : 'Dosya seçilmedi';
        $(this).next('.custom-file-label').text(fileName);
    });
    
    // Select2 Entegrasyonu (Kullanılıyorsa)
    if ($.fn.select2) {
        $('.select2').select2({
            theme: 'bootstrap4',
            language: 'tr',
            placeholder: 'Seçiniz...'
        });
    }
}); 