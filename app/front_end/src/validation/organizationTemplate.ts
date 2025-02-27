import * as yup from 'yup';

export const OrganizationTemplateValidation = yup.object({
  name: yup
    .string()
    .required('Name is required'),
  description: yup
    .string()
    .required('Description is required'),
});