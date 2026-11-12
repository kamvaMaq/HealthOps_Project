// Professional Global Scripts for HealthOps Project
class ProfessionalUI {
    static init() {
        this.initSearch();
        this.initFilters();
        this.initInteractions();
        this.initAutoRefresh();
    }

    static initSearch() {
        let searchTimeout;
        $(document).ready(function () {
            $('.professional-search-box').on('keyup', function () {
                clearTimeout(searchTimeout);
                searchTimeout = setTimeout(() => {
                    const query = $(this).val().toLowerCase();
                    const tableBody = $(this).closest('.professional-glass-card').find('.professional-table-row');
                    let visibleRows = 0;

                    tableBody.each(function () {
                        const rowText = $(this).text().toLowerCase();
                        if (rowText.indexOf(query) > -1) {
                            $(this).slideDown(400);
                            visibleRows++;
                        } else {
                            $(this).slideUp(400);
                        }
                    });

                    ProfessionalUI.updateSearchStatistics(visibleRows);
                }, 350);
            });
        });
    }

    static initFilters() {
        document.querySelectorAll('.professional-filter-btn').forEach(btn => {
            btn.addEventListener('click', function () {
                document.querySelectorAll('.professional-filter-btn').forEach(b => b.classList.remove('active'));
                this.classList.add('active');

                const filter = this.dataset.filter;
                const tableRows = document.querySelectorAll('.professional-table-row');
                let visibleCount = 0;

                tableRows.forEach(row => {
                    const rowText = row.textContent.toLowerCase();
                    let shouldShow = false;

                    switch (filter) {
                        case 'all':
                            shouldShow = true;
                            break;
                        case 'active':
                        case 'normal':
                            shouldShow = rowText.includes('active') || rowText.includes('normal') || rowText.includes('operational');
                            break;
                        case 'inactive':
                        case 'warning':
                            shouldShow = rowText.includes('inactive') || rowText.includes('warning');
                            break;
                        case 'critical':
                            shouldShow = rowText.includes('critical');
                            break;
                        default:
                            shouldShow = rowText.includes(filter);
                    }

                    if (shouldShow) {
                        row.style.display = '';
                        visibleCount++;
                    } else {
                        row.style.display = 'none';
                    }
                });

                ProfessionalUI.updateFilterStatistics(filter, visibleCount);
            });
        });
    }

    static initInteractions() {
        // Ripple effect for buttons
        document.querySelectorAll('.btn-professional-action').forEach(button => {
            button.addEventListener('click', function (e) {
                const ripple = document.createElement('span');
                const rect = this.getBoundingClientRect();
                const size = Math.max(rect.width, rect.height);
                const x = e.clientX - rect.left - size / 2;
                const y = e.clientY - rect.top - size / 2;

                ripple.style.cssText = `
                    position: absolute;
                    border-radius: 50%;
                    background: rgba(255, 255, 255, 0.6);
                    transform: scale(0);
                    animation: professionalRipple 0.6s linear;
                    width: ${size}px;
                    height: ${size}px;
                    left: ${x}px;
                    top: ${y}px;
                `;

                this.appendChild(ripple);
                setTimeout(() => ripple.remove(), 600);
            });
        });

        // Hover effects
        document.querySelectorAll('.professional-stat-card').forEach(card => {
            card.addEventListener('mouseenter', () => {
                card.style.transform = 'translateY(-6px) scale(1.02)';
            });
            card.addEventListener('mouseleave', () => {
                card.style.transform = 'translateY(0) scale(1)';
            });
        });

        // Search box interactions
        document.querySelectorAll('.professional-search-box').forEach(box => {
            box.addEventListener('focus', () => {
                box.style.transform = 'translateY(-2px)';
                box.style.boxShadow = '0 12px 30px rgba(106, 17, 203, 0.25)';
            });
            box.addEventListener('blur', () => {
                box.style.transform = 'translateY(0)';
                box.style.boxShadow = '0 6px 20px rgba(0, 0, 0, 0.08)';
            });
        });
    }

    static initAutoRefresh() {
        setInterval(() => {
            console.log('🔄 Professional auto-refresh: Checking for updates...');
            ProfessionalUI.updateLiveIndicators();
        }, 45000);
    }

    static updateSearchStatistics(visibleCount) {
        console.log(`🔍 Professional search: ${visibleCount} items match your criteria`);
    }

    static updateFilterStatistics(filter, count) {
        console.log(`🎯 Professional filter "${filter}": Showing ${count} items`);
    }

    static updateLiveIndicators() {
        document.querySelectorAll('.professional-pulse').forEach(element => {
            element.style.animation = 'none';
            setTimeout(() => {
                element.style.animation = 'professionalPulse 2.5s infinite';
            }, 10);
        });
    }
}

// Initialize when document is ready
document.addEventListener('DOMContentLoaded', function () {
    ProfessionalUI.init();
});

// Add ripple animation to global styles
const professionalRippleStyle = document.createElement('style');
professionalRippleStyle.textContent = `
    @keyframes professionalRipple {
        to {
            transform: scale(4);
            opacity: 0;
        }
    }
`;
document.head.appendChild(professionalRippleStyle);