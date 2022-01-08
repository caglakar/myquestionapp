import React from 'react';
import {
  FieldContainer,
  FieldError,
  FieldInput,
  FieldLabel,
  FieldTextArea,
  Fieldset,
  PrimaryButton,
  FormButtonContainer,
  SubmissionSuccess,
} from './Styles';
import { Page } from './Page';
import { useForm } from 'react-hook-form';
import { postQuestion } from './QuestionData';

type FormData = {
  title: string;
  content: string;
};
export const AskPage = () => {
  const [successfullySubmitted, setSuccessfullySubmitted] =
    React.useState(false);
  const {
    register,
    formState: { errors, isSubmitting },
    handleSubmit,
  } = useForm<FormData>({ mode: 'onBlur' });
  const submitForm = async (data: FormData) => {
    const result = await postQuestion({
      title: data.title,
      content: data.content,
      created: new Date(),
      userName: 'Me',
    });
    setSuccessfullySubmitted(result ? true : false);
  };
  return (
    <Page title="Ask a question">
      <form onSubmit={handleSubmit(submitForm)}>
        <Fieldset disabled={isSubmitting || successfullySubmitted}>
          <FieldContainer>
            <FieldLabel htmlFor="title">Title</FieldLabel>
            <FieldInput
              id="title"
              {...register('title', { required: true, minLength: 10 })}
              name="title"
              type="text"
            />
            {errors.title && errors.title.type === 'required' && (
              <FieldError>You must enter the question title</FieldError>
            )}
            {errors.title && errors.title.type === 'minLenght' && (
              <FieldError>The title must be at least 10 characters</FieldError>
            )}
          </FieldContainer>
          <FieldContainer>
            <FieldLabel htmlFor="content">Content</FieldLabel>
            <FieldTextArea
              id="content"
              {...register('content', { required: true, minLength: 50 })}
              name="content"
            />
            {errors.content && errors.content.type === 'required' && (
              <FieldError>You must enter the question title</FieldError>
            )}
            {errors.content && errors.content.type === 'minLenght' && (
              <FieldError>The title must be at least 10 characters</FieldError>
            )}
          </FieldContainer>
          <FormButtonContainer>
            <PrimaryButton type="submit">Submit Your Question</PrimaryButton>
          </FormButtonContainer>
          {successfullySubmitted && (
            <SubmissionSuccess>
              Your question is successfully submitted.
            </SubmissionSuccess>
          )}
        </Fieldset>
      </form>
    </Page>
  );
};

export default AskPage;
