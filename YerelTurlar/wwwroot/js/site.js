// Please see documentation at https://learn.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

$(document).ready(function() {
    // Mobil menü toggle
    $('.mobile-menu-toggle').click(function(e) {
        e.stopPropagation();
        $('.mobile-menu').toggleClass('active');
        $('body').toggleClass('no-scroll');
    });
    
    // Dışarı tıklandığında mobil menüyü kapat
    $(document).click(function(e) {
        if (!$(e.target).closest('.mobile-menu').length && !$(e.target).closest('.mobile-menu-toggle').length) {
            $('.mobile-menu').removeClass('active');
            $('body').removeClass('no-scroll');
        }
    });
    
    // Ekran boyutu değiştiğinde mobil menüyü kapat
    $(window).resize(function() {
        if ($(window).width() > 992) {
            $('.mobile-menu').removeClass('active');
            $('body').removeClass('no-scroll');
        }
    });
    
    // Ensure mobile menu is hidden on page load for desktop
    if ($(window).width() > 992) {
        $('.mobile-menu').removeClass('active');
        $('body').removeClass('no-scroll');
    }
    
    // Mobil menüdeki linklere tıklandığında menüyü kapat
    $('.mobile-menu ul li a').click(function() {
        $('.mobile-menu').removeClass('active');
        $('body').removeClass('no-scroll');
    });

    // Hero Slider Functionality
    initHeroSlider();
    
    // Bildirim kapatma işlevi
    initNotifications();
    
    // Şifre gücü ölçer
    initPasswordStrengthMeter();
    
    // Arama formu input focus efekti
    $('.input-with-icon input, .input-with-icon select').focus(function() {
        $(this).closest('.input-with-icon').css('border-color', '#ff5a5f');
    }).blur(function() {
        if (!$(this).val()) {
            $(this).closest('.input-with-icon').css('border-color', '#eee');
        }
    });
});

// Hero Slider Functionality
function initHeroSlider() {
    // Slider değişkenleri
    let currentSlide = 0;
    const slides = $('.hero-slide');
    const dots = $('.dot');
    const totalSlides = slides.length;
    let slideInterval;

    if (slides.length === 0) return;

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
}

// Bildirim kapatma işlevi
function initNotifications() {
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
}

// Şifre gücü ölçer
function initPasswordStrengthMeter() {
    const passwordInput = $('#password');
    const strengthMeter = $('.strength-meter-fill');
    
    if (passwordInput.length && strengthMeter.length) {
        passwordInput.on('input', function() {
            const password = $(this).val();
            const strength = calculatePasswordStrength(password);
            
            strengthMeter.attr('data-strength', strength);
        });
    }
    
    function calculatePasswordStrength(password) {
        // Boş şifre
        if (!password) return 0;
        
        let strength = 0;
        
        // Uzunluk kontrolü
        if (password.length >= 8) strength += 1;
        
        // Karakter çeşitliliği kontrolleri
        if (/[A-Z]/.test(password)) strength += 1;
        if (/[a-z]/.test(password)) strength += 1;
        if (/[0-9]/.test(password)) strength += 1;
        if (/[^A-Za-z0-9]/.test(password)) strength += 1;
        
        return Math.min(4, strength);
    }
}
