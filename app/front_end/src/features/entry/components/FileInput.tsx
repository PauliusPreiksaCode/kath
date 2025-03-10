import { useTheme, Box, Typography } from '@mui/material';
import toastService from "@/services/toast";
import { useRef, useState } from "react";

type FileInputProps = {
    setFile: (file: string | ArrayBuffer | null) => void;
    setFileName: (fileName: string) => void;
};

function FileInput({ setFile, setFileName }: FileInputProps) {
    const inputRef = useRef<HTMLInputElement | null>(null);
    const Theme = useTheme();
    const [dragging, setDragging] = useState(false);

    const handleOnChange = (event: React.ChangeEvent<HTMLInputElement>) => {
        const file = event.target.files?.[0];
        if (file) processFile(file);
      };

      const handleDrop = (event: React.DragEvent<HTMLDivElement>) => {
        event.preventDefault();
        setDragging(false);
        const file = event.dataTransfer.files?.[0];
        if (file) processFile(file);
      };

      const processFile = (file: File) => {
        setFileName(file.name);
    
        const allowedTypes = [
          "image/jpeg",
          "image/png",
          "application/pdf",
          "application/vnd.openxmlformats-officedocument.wordprocessingml.document",
          "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
          "application/vnd.openxmlformats-officedocument.presentationml.presentation",
          "text/plain",
        ];
    
        if (!allowedTypes.includes(file.type)) {
          toastService.error("Invalid file type. Only JPG, PNG, PDF, DOCX, XLSX, PPTX, and TXT files are allowed.");
          return;
        }
    
        const reader = new FileReader();
        reader.readAsDataURL(file);
        reader.onload = () => {
          setFile(reader.result);
        };
    
        if (inputRef.current) inputRef.current.value = "";
      };

      return (
        <Box
          sx={{
            border: `2px dashed ${dragging ? Theme.palette.primary.main : "#ccc"}`,
            borderRadius: "1rem",
            padding: "2rem",
            textAlign: "center",
            cursor: "pointer",
            backgroundColor: dragging ? "#f0f0f0" : "transparent",
            transition: "background-color 0.3s ease",
          }}
          onClick={() => inputRef.current?.click()}
          onDragOver={(e) => {
            e.preventDefault();
            setDragging(true);
          }}
          onDragLeave={() => setDragging(false)}
          onDrop={handleDrop}
        >
          <input
            type="file"
            ref={inputRef}
            onChange={handleOnChange}
            accept=".jpg,.jpeg,.png,.pdf,.docx,.xlsx,.pptx,.txt"
            style={{ display: "none" }}
          />
    
          <Typography variant="body1" color="textSecondary">
            {dragging ? "Drop the file here..." : "Drag & Drop a file here or Click to select a file"}
          </Typography>
        </Box>
      );

}

export default FileInput;