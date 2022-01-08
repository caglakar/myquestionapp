/** @jsxImportSource @emotion/react */
import { css } from '@emotion/react';
import React from 'react';
import { QuestionData } from './QuestionData';
import { Question } from './Question';
import { accent2, gray5 } from './Styles';

interface MyPropsType {
  dataInMyProp: QuestionData[];
  renderItem?: (item: QuestionData) => JSX.Element; //renderItem, questionData parametresi alan JSX elementi dönen bir fonksiyon
}

//(props: MyPropsType) type annottaion
export const QuestionList = ({ dataInMyProp, renderItem }: MyPropsType) => (
  <ul
    css={css`
      list-style: none;
      margin: 10px 0 0 0;
      padding: 0px 20px;
      background-color: #fff;
      border-bottom-left-radius: 4px;
      border-bottom-right-radius: 4px;
      border-top: 3px solid ${accent2};
      box-shadow: 0 3px 5px 0 rgba(0, 0, 0, 0.16);
    `}
  >
    {dataInMyProp.map((question) => (
      <>
        <li
          key={question.questionId}
          css={css`
            border-top: 1px solid ${gray5};
            :first-of-type {
              border-top: none;
            }
          `}
        ></li>
        {renderItem ? renderItem(question) : <Question data={question} />}
      </>
    ))}
  </ul>
);
