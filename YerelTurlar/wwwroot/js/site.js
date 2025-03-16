// Please see documentation at https://learn.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Hero Slider Functionality
$(document).ready(function() {
    // Slider değişkenleri
    let currentSlide = 0;
    const slides = $('.hero-slide');
    const dots = $('.dot');
    const totalSlides = slides.length;
    let slideInterval;

    // Otomatik slider başlat
    startSlideShow();

    // Slider'ı başlat
    function startSlideShow() {
        slideInterval = setInterval(nextSlide, 5000); // 5 saniyede bir değiştir
    }

    // Slider'ı durdur
    function stopSlideShow() {
        clearInterval(slideInterval);
    }

    // Sonraki slide'a geç
    function nextSlide() {
        goToSlide((currentSlide + 1) % totalSlides);
    }

    // Önceki slide'a geç
    function prevSlide() {
        goToSlide((currentSlide - 1 + totalSlides) % totalSlides);
    }

    // Belirli bir slide'a git
    function goToSlide(slideIndex) {
        // Aktif slide'ı kaldır
        slides.removeClass('active');
        dots.removeClass('active');
        
        // Yeni slide'ı aktif yap
        $(slides[slideIndex]).addClass('active');
        $(dots[slideIndex]).addClass('active');
        
        // Geçerli slide'ı güncelle
        currentSlide = slideIndex;
    }

    // Slider kontrolleri için event listener'lar
    $('.slider-arrow.next').click(function() {
        stopSlideShow();
        nextSlide();
        startSlideShow();
    });

    $('.slider-arrow.prev').click(function() {
        stopSlideShow();
        prevSlide();
        startSlideShow();
    });

    $('.dot').click(function() {
        stopSlideShow();
        const slideIndex = $(this).data('slide');
        goToSlide(slideIndex);
        startSlideShow();
    });

    // Slider üzerine gelince otomatik geçişi durdur
    $('.hero-slider').hover(
        function() { stopSlideShow(); },
        function() { startSlideShow(); }
    );
    
    // Arama formu input focus efekti
    $('.input-with-icon input, .input-with-icon select').focus(function() {
        $(this).closest('.input-with-icon').css('border-color', '#ff5a5f');
    }).blur(function() {
        if (!$(this).val()) {
            $(this).closest('.input-with-icon').css('border-color', '#eee');
        }
    });
});

// Mobil menü için toggle fonksiyonu
$(document).ready(function() {
    $('.menu-icon').click(function() {
        $('.main-menu').toggleClass('active');
        $('.user-menu').toggleClass('active');
    });
});

// Bildirim kapatma işlevi
$(document).ready(function() {
    // Bildirim kapatma butonu tıklandığında
    $('.notification-close').click(function() {
        $(this).closest('.notification').fadeOut(300, function() {
            $(this).remove();
        });
    });
    
    // Bildirimleri otomatik olarak 5 saniye sonra kapat
    setTimeout(function() {
        $('.notification').fadeOut(300, function() {
            $(this).remove();
        });
    }, 5000);
});
