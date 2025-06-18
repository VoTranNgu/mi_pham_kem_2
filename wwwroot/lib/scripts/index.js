const listImage = document.querySelector('.list-image');
const imgs = document.querySelectorAll('.list-image img'); // More specific selection
const btnLeft = document.querySelector('.btn-left');
const btnRight = document.querySelector('.btn-right');
const length = imgs.length;
let current = 0;

const handleChangeSlide = () => {
    let width = listImage.offsetWidth; // Dynamically get the width of the container
    if (current === length - 1) {
        current = 0;
        listImage.style.transform = `translateX(0px)`; // Reset to the first image
    } else {
        current++;
        listImage.style.transform = `translateX(-${width * current}px)`; // Slide to the next image
    }
}

//setInterval(() => {
//    let width = listImage.offsetWidth; // Dynamically get the width of the container
//    if (current === length - 1) {
//        current = 0;
//        listImage.style.transform = `translateX(0px)`; // Reset to the first image
//    } else {
//        current++;
//        listImage.style.transform = `translateX(-${width * current}px)`; // Slide to the next image
//    }
//}, 4000)

let handleEventChangeSlide = setInterval(handleChangeSlide, 4000)

btnRight.addEventListener('click', () => {
    clearInterval(handleEventChangeSlide)
    handleChangeSlide()
    handleEventChangeSlide = setInterval(handleChangeSlide, 4000)
})

btnLeft.addEventListener('click', () => {
    clearInterval(handleEventChangeSlide); // Dừng auto slide khi bấm nút
    let width = listImage.offsetWidth; // Lấy chiều rộng của container
    if (current === 0) {
        current = length - 1; // Nếu đang ở ảnh đầu tiên, quay lại ảnh cuối cùng
    } else {
        current--; // Ngược lại, lùi về ảnh trước
    }
    listImage.style.transform = `translateX(-${width * current}px)`; // Cập nhật vị trí của slider
    handleEventChangeSlide = setInterval(handleChangeSlide, 4000); // Khởi động lại auto slide
});