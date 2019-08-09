export class ModalService {
    private popupWrapper: HTMLElement;
    private content: HTMLElement;
    private template = `<div class="popup-wrapper">
                                <div class="popup">
                                    <div class="content">
    
                                    </div>
                                    <a class="close">x</a>
                                </div>
                            </div>`; 
    
    constructor() {
        this.popupWrapper = this.createHtmlElement(this.template);
        this.content = this.popupWrapper.querySelector(".content");
    
        const closeButton = this.popupWrapper.querySelector(".close");
        closeButton.addEventListener('click', () => this.onClose());

        this.popupWrapper.addEventListener("animationend", x => {
            if (x.animationName === "fadeOut") {
                this.content.innerHTML = "";
                this.popupWrapper.classList.remove("closing");
                this.popupWrapper.remove();
            }
        });
    }
    
    showModal(template: string, viewModel: any) {
        const content = this.createHtmlElement(template);
        ko.applyBindings(viewModel, content);
        this.content.appendChild(content);
        document.body.appendChild(this.popupWrapper);
    }
    
    private createHtmlElement(template: string) {
        const tempElement = document.createElement("div");
        tempElement.innerHTML = template;
        return <HTMLElement>tempElement.firstElementChild;
    }
    private onClose() {
        // this.popupWrapper.remove();
        // this.content.innerHTML = "";
        this.popupWrapper.classList.add("closing");
    }
}