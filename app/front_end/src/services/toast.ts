import { toast } from "react-toastify";

const error = (message: string) => {
    toast.error(message);
    toast.clearWaitingQueue();
}

const success = (message : string) => {
    toast.success(message);
    toast.clearWaitingQueue();
};

const toastService = {
    error,
    success,
};

export default toastService;
